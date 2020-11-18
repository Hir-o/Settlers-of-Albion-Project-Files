using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lean.Gui;
using NaughtyAttributes;
using TbsFramework.Grid.GridStates;
using TbsFramework.Units;
using UnityEngine;

public class Building : MonoBehaviour
{
    public bool isSettlement, isStronghold, isEnemyControlled;

    [ShowIf("isSettlement")] [BoxGroup("GFX")]
    [SerializeField] private GameObject _gfxHamlet, _gfxVillage, _gfxTown, _gfxCity;

    [ShowIf("isStronghold")] [BoxGroup("GFX")]
    [SerializeField] private GameObject _gfxOutpost, _gfxCastle;

    [BoxGroup("Resources Gain")]
    public int gold, wood, grain, sheep, stone, horse;

    [BoxGroup("Cells")]
    [SerializeField] private float _radius;

    [BoxGroup("Cells")]
    [SerializeField] private LayerMask _tileMask;

    [BoxGroup("Cells")]
    [SerializeField] private HexagonTile _tiles;

    [BoxGroup("Sieging Enemies")]
    [SerializeField] private LayerMask _enemyMask;

    [BoxGroup("Sieging Enemies")]
    [SerializeField] private Collider2D[] _siegingEnemiesColliders;

    [BoxGroup("Sieging Enemies")]
    [SerializeField] private List<UnitController> _siegingEnemies = new List<UnitController>();

    [BoxGroup("Village Upgrade Cost")]
    [SerializeField] private int _villageCostGold, _villageCostWood, _villageCostGrain;

    [BoxGroup("Town Upgrade Cost")]
    [SerializeField] private int _townCostGold, _townCostWood, _townCostGrain, _townCostStone;

    [BoxGroup("City Upgrade Cost")]
    [SerializeField] private int _cityCostGold, _cityCostWood, _cityCostGrain, _cityCostSheep, _cityCostStone;

    [BoxGroup("Castle Upgrade Cost")]
    [SerializeField] private int _castleCostGold, _castleCostWood, _castleCostSheep, _castleCostStone;

    [SerializeField] private Collider2D[]      _hexagonColliders;
    [SerializeField] private List<HexagonTile> _buildingTiles           = new List<HexagonTile>();
    [SerializeField] private List<HexagonTile> _adjacentSettlementTiles = new List<HexagonTile>();

    private UnitController      _unitController;
    private HexagonTile         _buildingTile;
    private ResourcesController _resourcesController;
    private Dockyard            _dockyard;

    public bool checkForQuestsAtStart = true;

    public enum UpgradeLevel
    {
        None,
        Hamlet,
        Village,
        Town,
        City
    }

    public enum StrongholdUpgradeLevel
    {
        None,
        Outpost,
        Castle
    }

    public UpgradeLevel           upgradeLevel           = UpgradeLevel.Hamlet;
    public StrongholdUpgradeLevel strongholdUpgradeLevel = StrongholdUpgradeLevel.None;
    
    private void Start()
    {
        _resourcesController = ResourcesController.Instance;

        if (isEnemyControlled == false)
        {
            if (isSettlement)
            {
                Statistics.Instance.spawnedSettlements++;
                upgradeLevel = UpgradeLevel.Hamlet;
                UpdateGFX();
            }
            else if (isStronghold)
            {
                Statistics.Instance.spawnedStrongholds++;
                upgradeLevel = UpgradeLevel.None;
            }
        }

        if (isSettlement || isStronghold)
        {
            _unitController = GetComponent<UnitController>();
            _unitController.SetBuilding(this);

            if (isStronghold) gold = -Cost.Instance.outpostGoldUpkeep;

            _buildingTile = _unitController.Cell.GetComponent<HexagonTile>();

            if (_buildingTile.isSettlementTile) _buildingTile.isSettlementTile = false;

            if (_buildingTile.settlement != null) _buildingTile.settlement._buildingTiles.Remove(_buildingTile);

            AddTerrainModifiers();
        }

        ObjectHolder.Instance.buildings.Add(this);

        if (isEnemyControlled) UpdateEnemyBuildingGFX();

        GetAdjacentTiles();

        _resourcesController.UpdateOnlyResourceUIText();

        CheckForSiegingEnemies();

        if (isEnemyControlled == false && checkForQuestsAtStart)
        {
            Quests.Instance.CheckWinConditions();
            Quests.Instance.CheckOptionalQuests();
        }
    }

    private void GetAdjacentTiles()
    {
        HexagonTile tempTile;

        _hexagonColliders = Physics2D.OverlapCircleAll(transform.position, _radius, _tileMask);

        foreach (Collider2D col in _hexagonColliders)
        {
            tempTile = col.GetComponent<HexagonTile>();

            if (!ReferenceEquals(tempTile, _unitController.Cell))
            {
                if (tempTile.IsTaken && tempTile.CurrentUnit.unitType == Unit.UnitType.Building)
                {
                    _adjacentSettlementTiles.Add(tempTile);
                    continue;
                }

                if (isEnemyControlled)
                {
                    if (tempTile.isSettlementTile == false)
                    {
                        _buildingTiles.Add(tempTile);
                        tempTile.isSettlementTile = true;
                        tempTile.settlement       = this;
                    }
                }

                if (isSettlement)
                {
                    if (tempTile.isSettlementTile == false && tempTile.isHazard == false)
                    {
                        if (tempTile.isDockTile)
                        {
                            _dockyard = tempTile.dockyard;
                            ObjectHolder.Instance.dockyards.Add(_dockyard);
                            tempTile.isSettlementTile = true;
                            tempTile.settlement       = this;

                            Cost.Instance.UpdateMarketPriceFee();
                            ObjectHolder.Instance.UpdateAllButtonStates();

                            continue;
                        }

                        _buildingTiles.Add(tempTile);
                        tempTile.isSettlementTile = true;
                        tempTile.settlement       = this;
                    }
                }
                else if (isStronghold) { _buildingTiles.Add(tempTile); }
            }
        }
    }

    public void CheckForSiegingEnemies()
    {
        if (isEnemyControlled) return;

        if (isSettlement == false && isStronghold == false) return;

        UnitController enemy;

        _siegingEnemiesColliders = Physics2D.OverlapCircleAll(transform.position, _radius, _enemyMask);
        _siegingEnemies.Clear();

        foreach (Collider2D col in _siegingEnemiesColliders)
        {
            enemy = col.GetComponent<UnitController>();

            if (enemy.PlayerNumber != 0) _siegingEnemies.Add(enemy);
        }

        if (_siegingEnemies.Count > 0)
            _unitController.GetHealthbar().UpdateUnderSiegeIcon(true);
        else
            _unitController.GetHealthbar().UpdateUnderSiegeIcon(false);

        if (_unitController.HitPoints < _unitController.MaxHitPoints)
        {
            if (_siegingEnemies.Count > 0)
                _unitController.SetIsHealing(false);

            else
                _unitController.SetIsHealing(true);
        }
        else
            _unitController.SetIsHealing(false);
    }

    public void ShowBuildButtons()
    {
        foreach (HexagonTile tile in _buildingTiles) tile.ShowResourceButton();
    }

    public void HideBuildButtons()
    {
        foreach (HexagonTile tile in _buildingTiles) tile.HideResourceButtons();
    }

    public void Upgrade(LeanHover leanHover)
    {
        if (isSettlement && upgradeLevel != UpgradeLevel.City && isEnemyControlled == false)
        {
            int currentLevel = (int) upgradeLevel;
            currentLevel++;
            upgradeLevel = (UpgradeLevel) currentLevel;
            UpdateGFX();

            BuildingUIUpdater.Instance.UpdateResources();
            BuildingUIUpdater.Instance.CheckForBuildingButtons();
            _resourcesController.UpdateHorseResource();
            _resourcesController.UpdateOnlyResourceUIText();

            UpdateStats();

            ObjectHolder.Instance.upgradeUnitButtons.ForEach(x => x.CheckBuildingLevel());
            Quests.Instance.CheckWinConditions();
            Quests.Instance.CheckOptionalQuests();

            leanHover.ManualPointerExit();

            if (upgradeLevel       != UpgradeLevel.City &&
                (int) upgradeLevel < LevelController.Instance.maxSettlementUpgradeLevel)
                leanHover.ManualPointerEnter();
        }
    }
    
    public void Upgrade()
    {
        if (isSettlement && upgradeLevel != UpgradeLevel.City && isEnemyControlled == false)
        {
            int currentLevel = (int) upgradeLevel;
            currentLevel++;
            upgradeLevel = (UpgradeLevel) currentLevel;
            UpdateGFX();

//            BuildingUIUpdater.Instance.UpdateResources();
//            BuildingUIUpdater.Instance.CheckForBuildingButtons();
            _resourcesController.UpdateHorseResource();
            _resourcesController.UpdateOnlyResourceUIText();

            UpdateStats();

            ObjectHolder.Instance.upgradeUnitButtons.ForEach(x => x.CheckBuildingLevel());
            Quests.Instance.CheckWinConditions();
            Quests.Instance.CheckOptionalQuests();
        }
    }

    public void UpgradeStronghold(LeanHover leanHover)
    {
        if (isStronghold && strongholdUpgradeLevel != StrongholdUpgradeLevel.Castle && isEnemyControlled == false)
        {
            int currentStrongholdLevel = (int) strongholdUpgradeLevel;
            currentStrongholdLevel++;
            strongholdUpgradeLevel = (StrongholdUpgradeLevel) currentStrongholdLevel;
            UpdateStrongHold();

            gold = -Cost.Instance.castleGoldUpkeep;

            BuildingUIUpdater.Instance.UpdateResources();
            BuildingUIUpdater.Instance.CheckForBuildingButtons();
            _resourcesController.UpdateOnlyResourceUIText();

            UpdateStrongholdStats();

            ObjectHolder.Instance.upgradeUnitButtons.ForEach(x => x.CheckBuildingLevel());
            leanHover.ManualPointerExit();

            if (strongholdUpgradeLevel != StrongholdUpgradeLevel.Castle) leanHover.ManualPointerEnter();
        }
    }

    private void UpdateStats()
    {
        int hitPointsDifference = _unitController.MaxHitPoints - _unitController.HitPoints;

        switch (upgradeLevel)
        {
            case UpgradeLevel.Village:
                _unitController.MaxHitPoints = _unitController.InitMaxHitPoints * 2;
                _unitController.HitPoints    = _unitController.MaxHitPoints - hitPointsDifference;
                _unitController.UpdateHealthbar();
                break;
            case UpgradeLevel.Town:
                _unitController.MaxHitPoints = _unitController.InitMaxHitPoints * 4;
                _unitController.HitPoints    = _unitController.MaxHitPoints - hitPointsDifference;
                _unitController.UpdateHealthbar();
                break;
            case UpgradeLevel.City:
                _unitController.MaxHitPoints = _unitController.InitMaxHitPoints * 8;
                _unitController.HitPoints    = _unitController.MaxHitPoints - hitPointsDifference;
                _unitController.UpdateHealthbar();
                break;
        }
    }

    private void UpdateStrongholdStats()
    {
        int hitPointsDifference = _unitController.MaxHitPoints - _unitController.HitPoints;

        if (strongholdUpgradeLevel == StrongholdUpgradeLevel.Castle)
        {
            _unitController.MaxHitPoints = _unitController.InitMaxHitPoints * 2;
            _unitController.HitPoints    = _unitController.MaxHitPoints - hitPointsDifference;

            _unitController.AttackRange   += 1;
            _unitController.AttackFactor  += _unitController.AttackFactor / 2;
            _unitController.DefenceFactor *= 2;

            AddTerrainModifiers();

            ObjectHolder.Instance.cellGrid.CellGridState =
                new CellGridStateUnitSelected(ObjectHolder.Instance.cellGrid, _unitController);
            
            _unitController.UpdateHealthbar();
        }
    }

    private void AddTerrainModifiers()
    {
        if (_unitController.Cell.GetComponent<HexagonTile>().isMountain &&
            _unitController.unitType == Unit.UnitType.Building)
        {
            if (isStronghold) _unitController.AttackRange += 1;
        }
    }

    public void UpdateEnemyBuildingGFX()
    {
        switch (upgradeLevel)
        {
            case UpgradeLevel.Hamlet:
                _gfxHamlet.SetActive(true);
                _gfxVillage.SetActive(false);
                _gfxTown.SetActive(false);
                _gfxCity.SetActive(false);
                break;
            case UpgradeLevel.Village:
                _gfxHamlet.SetActive(false);
                _gfxVillage.SetActive(true);
                _gfxTown.SetActive(false);
                _gfxCity.SetActive(false);
                break;
            case UpgradeLevel.Town:
                _gfxHamlet.SetActive(false);
                _gfxVillage.SetActive(false);
                _gfxTown.SetActive(true);
                _gfxCity.SetActive(false);
                break;
            case UpgradeLevel.City:
                _gfxHamlet.SetActive(false);
                _gfxVillage.SetActive(false);
                _gfxTown.SetActive(false);
                _gfxCity.SetActive(true);
                break;
        }
    }
    
    private void UpdateGFX()
    {
        if (isSettlement)
        {
            switch (upgradeLevel)
            {
                case UpgradeLevel.Hamlet:
                    _gfxHamlet.SetActive(true);
                    break;
                case UpgradeLevel.Village:
                    _gfxVillage.SetActive(true);
                    _gfxHamlet.SetActive(false);
                    break;
                case UpgradeLevel.Town:
                    _gfxTown.SetActive(true);
                    _gfxVillage.SetActive(false);
                    break;
                case UpgradeLevel.City:
                    _gfxCity.SetActive(true);
                    _gfxTown.SetActive(false);
                    break;
            }
        }
    }

    private void UpdateStrongHold()
    {
        if (isStronghold)
        {
            switch (strongholdUpgradeLevel)
            {
                case StrongholdUpgradeLevel.Outpost:
                    _gfxOutpost.SetActive(true);
                    break;
                case StrongholdUpgradeLevel.Castle:
                    _gfxOutpost.SetActive(false);
                    _gfxCastle.SetActive(true);
                    break;
            }
        }
    }

    public void HighlightTiles()
    {
        HideBuildButtons();

        foreach (HexagonTile tile in _buildingTiles)
        {
            if (tile.IsTaken == false && tile.groundType == GroundType.Land)
            {
                tile.isUnitSpawnable = true;
                tile.MarkAsHighlighted();
            }
        }
    }

    public void ResetHighlightedTile()
    {
        foreach (HexagonTile tile in _buildingTiles)
        {
            if (tile.isUnitSpawnable)
            {
                tile.isUnitSpawnable = false;
                tile.UnMark();
            }
        }
    }

    public UnitController GetUnitController() { return _unitController; }

    public List<HexagonTile> GetBuildingTiles() { return _buildingTiles; }

    private void OnDestroy()
    {
        foreach (HexagonTile tile in _buildingTiles)
        {
            if (tile.settlement != null) tile.settlement = null;

            if (tile.isSettlementTile) tile.isSettlementTile = false;

            tile.DestroyResourceBuilding();
        }

        if (_dockyard != null)
        {
            ObjectHolder.Instance.dockyards.Remove(_dockyard);
            Cost.Instance.UpdateMarketPriceFee();
            ObjectHolder.Instance.UpdateAllButtonStates();
        }

        ObjectHolder.Instance.buildings.Remove(this);
        Quests.Instance.CheckWinConditions();

        foreach (var b in ObjectHolder.Instance.buildings.Where(b => b != null)) b.GetAdjacentTiles();

        if (isEnemyControlled == false)
        {
            if (isSettlement)
                Statistics.Instance.spawnedSettlements--;
            else if (isStronghold)
                Statistics.Instance.spawnedStrongholds--;
        }
        
        Quests.Instance.CheckOptionalQuests();
        Quests.Instance.CheckLoseConditions();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}