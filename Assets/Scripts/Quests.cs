using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TbsFramework.Grid;
using TbsFramework.Units;
using UnityEngine;
using DG.Tweening;

public class Quests : MonoBehaviour
{
    private static Quests _instance;
    public static  Quests Instance => _instance;

    public enum MainQuestType
    {
        Development,
        Battle,
        Defend
    }

    public MainQuestType mainQuestType = MainQuestType.Development;

    public int developmentPointsToWin = 10, currentDevelopmentPoints, reinforcementsAfterWave;

    public bool isGameWon, isGameLost, hasMainArmyArrived;

    [SerializeField] private List<Unit> _enemies = new List<Unit>();

    [BoxGroup("Side Quests")]
    public SideQuest[] sideQuests = new SideQuest[3];

    private List<Building> buildings = new List<Building>();
    private List<Unit> remainingUnits = new List<Unit>();

    private float _safeTimer = 1f;
    private bool _skipLoseCheck = true;
    
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        
        Invoke(nameof(EnableLosing), _safeTimer);
    }

    private void EnableLosing()
    {
        _skipLoseCheck = false;
    }

    public void CheckWinConditions()
    {
        switch (mainQuestType)
        {
            case MainQuestType.Development:
                currentDevelopmentPoints = 0;

                foreach (Building b in ObjectHolder.Instance.buildings)
                    if (b.isSettlement && b.GetUnitController().PlayerNumber == 0)
                        currentDevelopmentPoints += (int) b.upgradeLevel;

                if (currentDevelopmentPoints >= developmentPointsToWin)
                {
                    isGameWon = true;
                    QuestUIUpdater.Instance.ShowFinishPanel(isGameWon);
                }

                CheckLoseConditions();

                QuestUIUpdater.Instance.UpdateMainQuest();
                break;
            case MainQuestType.Battle:

                CheckForRemainingEnemies();

                if (_enemies.Count <= 0)
                {
                    isGameWon = true;
                    QuestUIUpdater.Instance.ShowFinishPanel(isGameWon);
                }

                CheckLoseConditions();

                QuestUIUpdater.Instance.UpdateMainQuest();
                break;
            case MainQuestType.Defend:

                if (hasMainArmyArrived)
                {
                    CheckForRemainingEnemies();

                    if (_enemies.Count <= 0)
                    {
                        isGameWon = true;
                        QuestUIUpdater.Instance.ShowFinishPanel(isGameWon);
                    }
                }

                CheckLoseConditions();
                QuestUIUpdater.Instance.UpdateMainQuest();

                if (reinforcementsAfterWave > 0) return;
                break;
        }
    }

    private void CheckForRemainingEnemies()
    {
        _enemies.Clear();
        _enemies = ObjectHolder.Instance.cellGrid.Units.FindAll(e => (e.PlayerNumber == 1) &&
                                                                     (e.unitType !=
                                                                      Unit.UnitType.Building));
    }

    public void CheckLoseConditions()
    {
        if (_skipLoseCheck) return;
        
        remainingUnits = ObjectHolder.Instance.cellGrid.Units.FindAll(e => e.PlayerNumber == 0);

        buildings.Clear();

        foreach (var unit in remainingUnits)
        {
            var unitController = (UnitController) unit;

            if (unitController.GetBuilding() != null && unitController.GetBuilding().isSettlement)
                buildings.Add(unitController.GetBuilding());
        }

        switch (mainQuestType)
        {
            case MainQuestType.Development:
                if (remainingUnits.Count <= 0)
                {
                    QuestUIUpdater.Instance.ShowFinishPanel(isGameWon);
                    isGameLost = true;
                }

                break;
            case MainQuestType.Battle:
                if (remainingUnits.Count <= 0)
                {
                    QuestUIUpdater.Instance.ShowFinishPanel(isGameWon);
                    isGameLost = true;
                }

                break;
            case MainQuestType.Defend:
                if (buildings.Count <= 0)
                {
                    QuestUIUpdater.Instance.ShowFinishPanel(isGameWon);
                    isGameLost = true;
                }

                break;
        }
    }

    public void CheckOptionalQuests()
    {
        foreach (SideQuest sideQuest in sideQuests)
        {
            if (sideQuest.isComplete) continue;
            if (sideQuest.isFailed) continue;

            switch (sideQuest.questType)
            {
                case SideQuest.SideQuestType.ReachDevelopment:
                    if (sideQuest.hasTurnLimit)
                    {
                        if (currentDevelopmentPoints                 >= sideQuest.questValueToComplete &&
                            ObjectHolder.Instance.cellGrid.turnCount < sideQuest.turnLimit)
                        {
                            sideQuest.isComplete              =  true;
                            ResourcesController.Instance.gold += sideQuest.goldReward;
                        }
                        else if (currentDevelopmentPoints                 < sideQuest.questValueToComplete &&
                                 ObjectHolder.Instance.cellGrid.turnCount >= sideQuest.turnLimit)
                            sideQuest.isFailed = true;
                    }
                    else
                    {
                        if (currentDevelopmentPoints >= sideQuest.questValueToComplete)
                        {
                            sideQuest.isComplete              =  true;
                            ResourcesController.Instance.gold += sideQuest.goldReward;
                        }
                    }

                    break;
                case SideQuest.SideQuestType.CollectResources:
                    int resourceAmount = 0;
                    switch (sideQuest.resourceType)
                    {
                        case SideQuest.Resource.Wood:
                            resourceAmount = ResourcesController.Instance.wood;
                            break;
                        case SideQuest.Resource.Grain:
                            resourceAmount = ResourcesController.Instance.grain;
                            break;
                        case SideQuest.Resource.Sheep:
                            resourceAmount = ResourcesController.Instance.sheep;
                            break;
                        case SideQuest.Resource.Stone:
                            resourceAmount = ResourcesController.Instance.stone;
                            break;
                    }

                    if (sideQuest.hasTurnLimit)
                    {
                        if (resourceAmount                           >= sideQuest.questValueToComplete &&
                            ObjectHolder.Instance.cellGrid.turnCount < sideQuest.turnLimit)
                        {
                            sideQuest.isComplete              =  true;
                            ResourcesController.Instance.gold += sideQuest.goldReward;
                        }
                        else if (resourceAmount                           < sideQuest.questValueToComplete &&
                                 ObjectHolder.Instance.cellGrid.turnCount >= sideQuest.turnLimit)
                            sideQuest.isFailed = true;
                    }
                    else
                    {
                        if (resourceAmount >= sideQuest.questValueToComplete)
                        {
                            sideQuest.isComplete              =  true;
                            ResourcesController.Instance.gold += sideQuest.goldReward;
                        }
                    }

                    break;
                case SideQuest.SideQuestType.EliminateEnemies:
                    int enemiesEliminated = 0;
                    switch (sideQuest.enemyType)
                    {
                        case SideQuest.Enemy.Boar:
                            enemiesEliminated = Statistics.Instance.boarsKilled;
                            break;
                        case SideQuest.Enemy.Wolf:
                            enemiesEliminated = Statistics.Instance.wolfsKilled;
                            break;
                        case SideQuest.Enemy.Werewolf:
                            enemiesEliminated = Statistics.Instance.werewolfsKilled;
                            break;
                        case SideQuest.Enemy.Bear:
                            enemiesEliminated = Statistics.Instance.bearsKilled;
                            break;
                        case SideQuest.Enemy.Slime:
                            enemiesEliminated = Statistics.Instance.slimesKilled;
                            break;
                        case SideQuest.Enemy.Goblin:
                            enemiesEliminated = Statistics.Instance.goblinsKilled;
                            break;
                        case SideQuest.Enemy.Orc:
                            enemiesEliminated = Statistics.Instance.orcsKilled;
                            break;
                        case SideQuest.Enemy.Frogman:
                            enemiesEliminated = Statistics.Instance.frogmansKilled;
                            break;
                        case SideQuest.Enemy.Troll:
                            enemiesEliminated = Statistics.Instance.trollsKilled;
                            break;
                        case SideQuest.Enemy.Cyclops:
                            enemiesEliminated = Statistics.Instance.cyclopsKilled;
                            break;
                        case SideQuest.Enemy.Yeti:
                            enemiesEliminated = Statistics.Instance.yetisKilled;
                            break;
                    }

                    if (sideQuest.hasTurnLimit)
                    {
                        if (enemiesEliminated                        >= sideQuest.questValueToComplete &&
                            ObjectHolder.Instance.cellGrid.turnCount < sideQuest.turnLimit)
                        {
                            sideQuest.isComplete              =  true;
                            ResourcesController.Instance.gold += sideQuest.goldReward;
                        }
                        else if (enemiesEliminated                        < sideQuest.questValueToComplete &&
                                 ObjectHolder.Instance.cellGrid.turnCount >= sideQuest.turnLimit)
                            sideQuest.isFailed = true;
                    }
                    else
                    {
                        if (enemiesEliminated >= sideQuest.questValueToComplete)
                        {
                            sideQuest.isComplete              =  true;
                            ResourcesController.Instance.gold += sideQuest.goldReward;
                        }
                    }

                    break;
                case SideQuest.SideQuestType.BuildOrRecruit:
                    int spawnedFriendly = 0;
                    switch (sideQuest.spawnableType)
                    {
                        case SideQuest.Spawnable.Villager:
                            spawnedFriendly = Statistics.Instance.spawnedVillagers;
                            break;
                        case SideQuest.Spawnable.Pikeman:
                            spawnedFriendly = Statistics.Instance.spawnedPikemans;
                            break;
                        case SideQuest.Spawnable.Archer:
                            spawnedFriendly = Statistics.Instance.spawnedArchers;
                            break;
                        case SideQuest.Spawnable.Footman:
                            spawnedFriendly = Statistics.Instance.spawnedFootmans;
                            break;
                        case SideQuest.Spawnable.Hussar:
                            spawnedFriendly = Statistics.Instance.spawnedHussars;
                            break;
                        case SideQuest.Spawnable.Knight:
                            spawnedFriendly = Statistics.Instance.spawnedKnights;
                            break;
                        case SideQuest.Spawnable.ArmouredArcher:
                            spawnedFriendly = Statistics.Instance.spawnedArmouredArchers;
                            break;
                        case SideQuest.Spawnable.MountedKnight:
                            spawnedFriendly = Statistics.Instance.spawnedMountedKnights;
                            break;
                        case SideQuest.Spawnable.Paladin:
                            spawnedFriendly = Statistics.Instance.spawnedPaladins;
                            break;
                        case SideQuest.Spawnable.MountedPaladin:
                            spawnedFriendly = Statistics.Instance.spawnedMountedPaladins;
                            break;
                        case SideQuest.Spawnable.King:
                            spawnedFriendly = Statistics.Instance.spawnedKings;
                            break;
                        case SideQuest.Spawnable.Settlement:
                            spawnedFriendly = Statistics.Instance.spawnedSettlements;
                            break;
                        case SideQuest.Spawnable.Stronghold:
                            spawnedFriendly = Statistics.Instance.spawnedStrongholds;
                            break;
                    }

                    if (sideQuest.hasTurnLimit)
                    {
                        if (spawnedFriendly                          >= sideQuest.questValueToComplete &&
                            ObjectHolder.Instance.cellGrid.turnCount < sideQuest.turnLimit)
                        {
                            sideQuest.isComplete              =  true;
                            ResourcesController.Instance.gold += sideQuest.goldReward;
                        }
                        else if (spawnedFriendly                          < sideQuest.questValueToComplete &&
                                 ObjectHolder.Instance.cellGrid.turnCount >= sideQuest.turnLimit)
                            sideQuest.isFailed = true;
                    }
                    else
                    {
                        if (spawnedFriendly >= sideQuest.questValueToComplete)
                        {
                            sideQuest.isComplete              =  true;
                            ResourcesController.Instance.gold += sideQuest.goldReward;
                        }
                    }

                    break;
                case SideQuest.SideQuestType.ControlPorts:
                    if (sideQuest.hasTurnLimit)
                    {
                        if (ObjectHolder.Instance.dockyards.Count    >= sideQuest.questValueToComplete &&
                            ObjectHolder.Instance.cellGrid.turnCount < sideQuest.turnLimit)
                        {
                            sideQuest.isComplete              =  true;
                            ResourcesController.Instance.gold += sideQuest.goldReward;
                        }
                        else if (ObjectHolder.Instance.dockyards.Count    < sideQuest.questValueToComplete &&
                                 ObjectHolder.Instance.cellGrid.turnCount >= sideQuest.turnLimit)
                            sideQuest.isFailed = true;
                    }
                    else
                    {
                        if (ObjectHolder.Instance.dockyards.Count >= sideQuest.questValueToComplete)
                        {
                            sideQuest.isComplete              =  true;
                            ResourcesController.Instance.gold += sideQuest.goldReward;
                        }
                    }

                    break;
            }

            if (sideQuest.isComplete) AudioController.Instance.SFXSideQuestCompleted();

            if (sideQuest.isFailed) AudioController.Instance.SFXSideQuestFailed();
        }

        QuestUIUpdater.Instance.UpdateSideQuests();
        ResourcesController.Instance.UpdateOnlyResourceUIText();
        ObjectHolder.Instance.UpdateAllButtonStates();
    }
}

[Serializable]
public class SideQuest
{
    public bool   isEnabled = true;
    public bool   hasTurnLimit;
    public int    turnLimit;
    public string sideQuestText;
    public int    goldReward;

    public SideQuestType questType     = SideQuestType.CollectResources;
    public Resource      resourceType  = Resource.None;
    public Enemy         enemyType     = Enemy.None;
    public Spawnable     spawnableType = Spawnable.None;

    public int questValueToComplete;

    [HideInInspector]
    public bool isComplete, isFailed;

    public enum SideQuestType
    {
        ReachDevelopment,
        CollectResources,
        EliminateEnemies,
        BuildOrRecruit,
        ControlPorts
    }

    public enum Resource
    {
        None,
        Wood,
        Grain,
        Sheep,
        Stone
    }

    public enum Enemy
    {
        None,
        Boar,
        Wolf,
        Werewolf,
        Bear,
        Slime,
        Goblin,
        Orc,
        Frogman,
        Troll,
        Cyclops,
        Yeti
    }

    public enum Spawnable
    {
        None,
        Villager,
        Pikeman,
        Archer,
        Footman,
        Hussar,
        Knight,
        ArmouredArcher,
        MountedKnight,
        Paladin,
        MountedPaladin,
        King,
        Settlement,
        Stronghold
    }
}