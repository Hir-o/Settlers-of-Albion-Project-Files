using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TbsFramework.Units;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;

public class ClickUnitButton : MonoBehaviour
{
    private Button    _btnSpawnUnit;
    private LeanHover _leanHover;

    [SerializeField] private UnitController _unit;
    [SerializeField] private GameObject     _unitGhost;

    private int costGold, costWood, costGrain, costSheep, costStone, costHorse, _settlersAndSettlements;

    private ResourcesController _resourcesController;
    private Cost                _cost;

    private void Awake()
    {
        _btnSpawnUnit = GetComponent<Button>();
        _leanHover    = GetComponent<LeanHover>();
        _btnSpawnUnit.onClick.AddListener(SpawnOnClick);
        _resourcesController = ResourcesController.Instance;
        _cost                = Cost.Instance;

        ObjectHolder.Instance.unitButtons.Add(this);
    }

    public void SpawnOnClick()
    {
        CalculateCosts();
        UpdateButtonState();

        if (ObjectHolder.Instance.purchasedUnitGhost != null)
            Destroy(ObjectHolder.Instance.purchasedUnitGhost.gameObject);

        BuildingUIUpdater.Instance.selectedBuilding.HighlightTiles();
        ObjectHolder.Instance.purchasedUnit = _unit;
        ObjectHolder.Instance.purchasedUnitGhost =
            Instantiate(_unitGhost, transform.position, Quaternion.Euler(0f, 0f, -90f));
        ObjectHolder.Instance.purchasedUnitGhost.SetActive(false);

        foreach (ClickUpgradeUnitButton btnUnitUpgrade in ObjectHolder.Instance.upgradeUnitButtons)
        {
            btnUnitUpgrade.CalculateCosts();
            btnUnitUpgrade.UpdateButtonState();
        }

        foreach (SpriteButton button in ObjectHolder.Instance.spriteButtons.Where(button => button != null))
            button.CalculateCosts();

        foreach (var button in ObjectHolder.Instance.marketBuyButtons) button.UpdateButtonState();

        foreach (var button in ObjectHolder.Instance.marketSellButtons) button.UpdateButtonState();

        if (ClickSettlementUpgradeButton.Instance != null)
        {
            ClickSettlementUpgradeButton.Instance.CalculateCosts();
            ClickSettlementUpgradeButton.Instance.UpdateButtonState();
        }

        AudioController.Instance.SFXButtonClick();
    }

    public void CalculateCosts()
    {
        _settlersAndSettlements = 0;

        foreach (Building building in ObjectHolder.Instance.buildings)
            if (building.isSettlement && building.GetUnitController().PlayerNumber == 0)
                _settlersAndSettlements++;

        foreach (UnitController unit in ObjectHolder.Instance.cellGrid.Units)
            if (unit.PlayerNumber == 0 && unit.unitType == Unit.UnitType.Settler)
                _settlersAndSettlements++;

        switch (_unit.unitType)
        {
            case Unit.UnitType.Settler:
                UpdateCostsVariables(_cost.goldSettler + (_settlersAndSettlements * _cost.goldSettlerMultiplier),
                                     _cost.woodSettler,  _cost.grainSettler, _cost.sheepSettler,
                                     _cost.stoneSettler, _cost.horseSettler);
                break;
            case Unit.UnitType.Villager:
                UpdateCostsVariables(_cost.goldVillager,  _cost.woodVillager, _cost.grainVillager, _cost.sheepVillager,
                                     _cost.stoneVillager, _cost.horseVillager);
                break;
            case Unit.UnitType.Pikeman:
                UpdateCostsVariables(_cost.goldPikeman,  _cost.woodPikeman, _cost.grainPikeman, _cost.sheepPikeman,
                                     _cost.stonePikeman, _cost.horsePikeman);
                break;
            case Unit.UnitType.Archer:
                UpdateCostsVariables(_cost.goldArcher,  _cost.woodArcher, _cost.grainArcher, _cost.sheepArcher,
                                     _cost.stoneArcher, _cost.horseArcher);
                break;
            case Unit.UnitType.Footman:
                UpdateCostsVariables(_cost.goldFootman,  _cost.woodFootman, _cost.grainFootman, _cost.sheepFootman,
                                     _cost.stoneFootman, _cost.horseFootman);
                break;
            case Unit.UnitType.Hussar:
                UpdateCostsVariables(_cost.goldHussar,  _cost.woodHussar, _cost.grainHussar, _cost.sheepHussar,
                                     _cost.stoneHussar, _cost.horseHussar);
                break;
            case Unit.UnitType.Knight:
                UpdateCostsVariables(_cost.goldKnight,  _cost.woodKnight, _cost.grainKnight, _cost.sheepKnight,
                                     _cost.stoneKnight, _cost.horseKnight);
                break;
            case Unit.UnitType.ArmouredArcher:
                UpdateCostsVariables(_cost.goldArmouredArcher, _cost.woodArmouredArcher, _cost.grainArmouredArcher,
                                     _cost.sheepArmouredArcher,
                                     _cost.stoneArmouredArcher, _cost.horseArmouredArcher);
                break;
            case Unit.UnitType.MountedKnight:
                UpdateCostsVariables(_cost.goldMountedKnight, _cost.woodMountedKnight, _cost.grainMountedKnight,
                                     _cost.sheepMountedKnight,
                                     _cost.stoneMountedKnight, _cost.horseMountedKnight);
                break;
            case Unit.UnitType.Paladin:
                UpdateCostsVariables(_cost.goldPaladin,  _cost.woodPaladin, _cost.grainPaladin, _cost.sheepPaladin,
                                     _cost.stonePaladin, _cost.horsePaladin);
                break;
            case Unit.UnitType.MountedPaladin:
                UpdateCostsVariables(_cost.goldMountedPaladin, _cost.woodMountedPaladin, _cost.grainMountedPaladin,
                                     _cost.sheepMountedPaladin,
                                     _cost.stoneMountedPaladin, _cost.horseMountedPaladin);
                break;
            case Unit.UnitType.King:
                UpdateCostsVariables(_cost.goldKing,  _cost.woodKing, _cost.grainKing, _cost.sheepKing,
                                     _cost.stoneKing, _cost.horseKing);
                break;
            case Unit.UnitType.Worker:
                UpdateCostsVariables(_cost.goldWorker,  _cost.woodWorker, _cost.grainWorker, _cost.sheepWorker,
                                     _cost.stoneWorker, _cost.horseWorker);
                break;
        }
    }

    public void UpdateButtonState()
    {
        switch (_unit.unitType)
        {
            case Unit.UnitType.Settler:
                if (LevelController.Instance.isSettlerEnabled == false) DisableButton();
                break;
            case Unit.UnitType.Villager:
                if (LevelController.Instance.isClubmanEnabled == false) DisableButton();
                break;
            case Unit.UnitType.Pikeman:
                if (LevelController.Instance.isPikemanEnabled == false) DisableButton();
                break;
            case Unit.UnitType.Archer:
                if (LevelController.Instance.isArcherEnabled == false) DisableButton();
                break;
            case Unit.UnitType.Footman:
                if (LevelController.Instance.isFootmanEnabled == false) DisableButton();
                break;
            case Unit.UnitType.Hussar:
                if (LevelController.Instance.isHussarEnabled == false) DisableButton();
                break;
            case Unit.UnitType.Knight:
                if (LevelController.Instance.isKnightEnabled == false) DisableButton();
                break;
            case Unit.UnitType.ArmouredArcher:
                if (LevelController.Instance.isArmouredArcherEnabled == false) DisableButton();
                break;
            case Unit.UnitType.MountedKnight:
                if (LevelController.Instance.isMountedKnightEnabled == false) DisableButton();
                break;
            case Unit.UnitType.Paladin:
                if (LevelController.Instance.isPaladinEnabled == false) DisableButton();
                break;
            case Unit.UnitType.MountedPaladin:
                if (LevelController.Instance.isMountedPaladinEnabled == false) DisableButton();
                break;
            case Unit.UnitType.King:
                if (LevelController.Instance.isKingEnabled == false) DisableButton();
                break;
            case Unit.UnitType.Worker:
                if (LevelController.Instance.isWorkerEnabled == false) DisableButton();
                break;
        }

        if (_resourcesController.gold  >= costGold  && _resourcesController.wood  >= costWood  &&
            _resourcesController.grain >= costGrain && _resourcesController.sheep >= costSheep &&
            _resourcesController.stone >= costStone &&
            _resourcesController.horse >= costHorse)
        {
            if (_btnSpawnUnit != null) _btnSpawnUnit.interactable = true;
        }
        else
        {
            if (_btnSpawnUnit != null) _btnSpawnUnit.interactable = false;
        }
    }

    private void UpdateCostsVariables(int gold, int wood, int grain, int sheep, int stone, int horse)
    {
        costGold  = _cost.unitToSpawnGold  = gold;
        costWood  = _cost.unitToSpawnWood  = wood;
        costGrain = _cost.unitToSpawnGrain = grain;
        costSheep = _cost.unitToSpawnSheep = sheep;
        costStone = _cost.unitToSpawnStone = stone;
        costHorse = _cost.unitToSpawnHorse = horse;
    }

    private void DisableButton()
    {
        if (_btnSpawnUnit != null) _btnSpawnUnit.transform.parent.gameObject.SetActive(false);

        _leanHover.ManualPointerExit();
    }

    public LeanHover GetLeanHover() { return _leanHover; }

    private void OnEnable()
    {
        CalculateCosts();
        UpdateButtonState();
    }
}