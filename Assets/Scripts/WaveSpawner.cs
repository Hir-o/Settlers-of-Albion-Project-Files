using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TbsFramework.Grid;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    private static WaveSpawner _instance;
    public static  WaveSpawner Instance => _instance;

    public bool isWaveLevel = true;

    public float turnsToPassForNewWave = 10f,
                 startingEnemyPoints,
                 enemyPointsMultiplier     = 1f,
                 enemyPointsToAddAfterWave = 10f,
                 waveCountDividerFactor    = 2f,
                 totalEnemyPoints;

    [BoxGroup("Enemy Points Costs")]
    public int wolfCost     = 3,
               werewolfCost = 5,
               slimeCost    = 2,
               goblinCost   = 5,
               orcCost      = 8,
               frogmanCost  = 12,
               trollCost    = 20,
               cyclopsCost  = 30,
               yetiCost     = 40;

    [BoxGroup("Turn Level Unlockers")]
    public int enemyLevel2WaveUnlocker = 10, enemyLevel3WaveUnlocker = 20, maxEnemyLevel;

    [BoxGroup("Turn Level Unlockers")]
    public bool allowLevel2Enemies, allowLevel3Enemies;

    private int _waveCount, _pointsPerWaveCount;

    [HideInInspector]
    public int turnsToNextWave;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        turnsToNextWave = (int) turnsToPassForNewWave;
    }

    public void StartNewEnemyTurn()
    {
        if (Quests.Instance.mainQuestType == Quests.MainQuestType.Defend)
        {
            if (Quests.Instance.reinforcementsAfterWave > 0)
            {
                Quests.Instance.reinforcementsAfterWave--;

                if (Quests.Instance.reinforcementsAfterWave == 0)
                {
                    foreach (var bridge in ObjectHolder.Instance.bridgeSpawners) { bridge.ActivateBridge(); }

                    foreach (var unit in ObjectHolder.Instance.specialUnitSpawners) { unit.SpawnUnit(); }
                }
            }
            else
                return;
        }

        turnsToNextWave--;

        if (ObjectHolder.Instance.cellGrid.turnCount % turnsToPassForNewWave == 0)
        {
            _waveCount++;

            if (_waveCount >= enemyLevel2WaveUnlocker && maxEnemyLevel < 1 && allowLevel2Enemies) maxEnemyLevel = 1;
            if (_waveCount >= enemyLevel3WaveUnlocker && maxEnemyLevel == 1 && maxEnemyLevel < 2 && allowLevel3Enemies)
                maxEnemyLevel = 2;

            if (_waveCount > 1)
                _pointsPerWaveCount = Mathf.CeilToInt(_waveCount / waveCountDividerFactor);
            else
                _pointsPerWaveCount = _waveCount;


            totalEnemyPoints = Mathf.RoundToInt(_pointsPerWaveCount                               *
                                                (startingEnemyPoints + enemyPointsToAddAfterWave) *
                                                enemyPointsMultiplier);

            int playerDevPoints = Quests.Instance.currentDevelopmentPoints / 2;

            totalEnemyPoints += playerDevPoints;

            foreach (EnemySpawner enemySpawner in ObjectHolder.Instance.enemySpawners) enemySpawner.SpawnUnits();

            turnsToNextWave = (int) turnsToPassForNewWave;

            if (DifficultyController.Instance != null)
            {
                DifficultyController.Instance.IncreaseEnemyPointsMultiplier(.1f);
                
                if (_waveCount == 1)
                    DifficultyController.Instance.IncreaseEnemyPointsMultiplier(.1f);
            
                if (_waveCount == 3)
                    DifficultyController.Instance.IncreaseEnemyPointsMultiplier(.1f);
            
                if (_waveCount == 4)
                    DifficultyController.Instance.IncreaseEnemyPointsMultiplier(.2f);
            
                if (_waveCount == 5)
                    DifficultyController.Instance.IncreaseEnemyPointsMultiplier(.4f);
                
                if (_waveCount == 6)
                    DifficultyController.Instance.IncreaseEnemyPointsMultiplier(.4f);
            }

            if (EventUiUpdater.Instance != null && EventUiUpdater.Instance.isEventEnabled)
                EventUiUpdater.Instance.showEvent = true;
        }
    }
}