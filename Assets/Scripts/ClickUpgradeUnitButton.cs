using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;

public class ClickUpgradeUnitButton : MonoBehaviour
{
    [SerializeField] private int _currentUpgradeLevel = 1;

    [BoxGroup("Rank Icons")]
    [SerializeField] private Image _imgInsignia1, _imgInsignia2;

    [BoxGroup("Rank Icons")]
    [SerializeField] private Color _goldColor;

    private Button              _btnUpgradeUnit;
    private UnitController      _unit;
    private CanvasGroup         _canvasGroup;
    private Building            _selectedBuilding;
    private Cost                _cost;
    private ResourcesController _resourcesController;
    private UnitLevelController _unitLevelController;
    private LeanHover           _leanHover;

    private int _costGold, _costWood, _costGrain, _costSheep, _costStone;

    private bool _isDisabled;

    public enum UnitToUpgrade
    {
        Pikeman,
        Archer,
        Footman,
        Hussar,
        Knight,
        ArmouredArcher,
        MountedKnight
    }

    public UnitToUpgrade unitToUpgrade = UnitToUpgrade.Pikeman;

    private void Awake()
    {
        ObjectHolder.Instance.upgradeUnitButtons.Add(this);

        _btnUpgradeUnit = GetComponent<Button>();
        _canvasGroup    = GetComponentInParent<CanvasGroup>();
        _leanHover      = GetComponent<LeanHover>();

        _cost                = Cost.Instance;
        _unitLevelController = UnitLevelController.Instance;
        _resourcesController = ResourcesController.Instance;

        _btnUpgradeUnit.onClick.AddListener(UpgradeUnit);
    }

    private void UpgradeUnit()
    {
        if (_isDisabled) return;

        CalculateCosts();
        UpdateButtonState();

        _selectedBuilding = BuildingUIUpdater.Instance.selectedBuilding;

        _currentUpgradeLevel++;

        switch (unitToUpgrade)
        {
            case UnitToUpgrade.Pikeman:
                _unitLevelController.pikemanLevel++;
                UpdateRank();
                CheckBuildingLevel();
                if (_unitLevelController.pikemanLevel >= 3)
                {
                    DisableButton();
                    _unitLevelController.pikemanLevel = 3;
                }

                if (_unitLevelController.pikemanLevel >= LevelController.Instance.maxUnitUpgradeLevel) DisableButton();

                break;
            case UnitToUpgrade.Archer:
                _unitLevelController.archerLevel++;
                UpdateRank();
                CheckBuildingLevel();
                if (_unitLevelController.archerLevel >= 3)
                {
                    DisableButton();
                    _unitLevelController.archerLevel = 3;
                }

                if (_unitLevelController.archerLevel >= LevelController.Instance.maxUnitUpgradeLevel) DisableButton();

                break;
            case UnitToUpgrade.Footman:
                _unitLevelController.footmanLevel++;
                UpdateRank();
                CheckBuildingLevel();
                if (_unitLevelController.footmanLevel >= 3)
                {
                    DisableButton();
                    _unitLevelController.footmanLevel = 3;
                }

                if (_unitLevelController.footmanLevel >= LevelController.Instance.maxUnitUpgradeLevel) DisableButton();
                break;
            case UnitToUpgrade.Hussar:
                _unitLevelController.hussarLevel++;
                UpdateRank();
                CheckBuildingLevel();
                if (_unitLevelController.hussarLevel >= 3)
                {
                    DisableButton();
                    _unitLevelController.hussarLevel = 3;
                }

                if (_unitLevelController.hussarLevel >= LevelController.Instance.maxUnitUpgradeLevel) DisableButton();
                break;
            case UnitToUpgrade.Knight:
                _unitLevelController.knightLevel++;
                UpdateRank();
                CheckBuildingLevel();
                if (_unitLevelController.knightLevel >= 3)
                {
                    DisableButton();
                    _unitLevelController.knightLevel = 3;
                }

                if (_unitLevelController.knightLevel >= LevelController.Instance.maxUnitUpgradeLevel) DisableButton();
                break;
            case UnitToUpgrade.ArmouredArcher:
                _unitLevelController.armouredArcherLevel++;
                UpdateRank();
                CheckBuildingLevel();
                if (_unitLevelController.armouredArcherLevel >= 3)
                {
                    DisableButton();
                    _unitLevelController.armouredArcherLevel = 3;
                }

                if (_unitLevelController.armouredArcherLevel >= LevelController.Instance.maxUnitUpgradeLevel)
                    DisableButton();
                break;
            case UnitToUpgrade.MountedKnight:
                _unitLevelController.mountedKnightLevel++;
                UpdateRank();
                CheckBuildingLevel();
                if (_unitLevelController.mountedKnightLevel >= 3)
                {
                    DisableButton();
                    _unitLevelController.mountedKnightLevel = 3;
                }

                if (_unitLevelController.mountedKnightLevel >= LevelController.Instance.maxUnitUpgradeLevel)
                    DisableButton();
                break;
        }

        _resourcesController.gold  -= _costGold;
        _resourcesController.wood  -= _costWood;
        _resourcesController.grain -= _costGrain;
        _resourcesController.sheep -= _costSheep;

        ResourcesController.Instance.UpdateOnlyResourceUIText();

        foreach (var unit in ObjectHolder.Instance.cellGrid.Units.Cast<UnitController>()
                                         .Where(unit => unit.PlayerNumber == 0))
            unit.UpdateUnitLevel();

        foreach (ClickUnitButton btnUnit in ObjectHolder.Instance.unitButtons)
        {
            btnUnit.CalculateCosts();
            btnUnit.UpdateButtonState();
        }

        foreach (ClickUpgradeUnitButton btnUnitUpgrade in ObjectHolder.Instance.upgradeUnitButtons)
        {
            btnUnitUpgrade.CalculateCosts();
            btnUnitUpgrade.UpdateButtonState();
        }

        foreach (var button in ObjectHolder.Instance.spriteButtons.Where(button => button != null))
            button.CalculateCosts();

        foreach (var button in ObjectHolder.Instance.marketBuyButtons) button.UpdateButtonState();

        foreach (var button in ObjectHolder.Instance.marketSellButtons) button.UpdateButtonState();

        if (ClickSettlementUpgradeButton.Instance != null)
        {
            ClickSettlementUpgradeButton.Instance.CalculateCosts();
            ClickSettlementUpgradeButton.Instance.UpdateButtonState();
        }
        
        AudioController.Instance.SFXUpgradeUnit();
    }

    private void UpdateRank()
    {
        if (_currentUpgradeLevel      > 2) { _leanHover.ManualPointerExit(); }
        else if (_currentUpgradeLevel > 1)
        {
            _imgInsignia1.color = _goldColor;
            _imgInsignia2.color = _goldColor;
            _leanHover.ManualPointerExit();
            _leanHover.ManualPointerEnter();
        }
    }

    public void CheckBuildingLevel()
    {
        _selectedBuilding = BuildingUIUpdater.Instance.selectedBuilding;

        if (_selectedBuilding.isSettlement)
        {
            if ((int) _selectedBuilding.upgradeLevel == 3 && _currentUpgradeLevel < 2)
                EnableButton();

            else if ((int) _selectedBuilding.upgradeLevel == 4 && _currentUpgradeLevel <= 2)
                EnableButton();
            else
                DisableButton();
        }
        else if (_selectedBuilding.isStronghold)
        {
            if ((int) _selectedBuilding.strongholdUpgradeLevel == 1 && _currentUpgradeLevel < 2)
                EnableButton();

            else if ((int) _selectedBuilding.strongholdUpgradeLevel == 2 && _currentUpgradeLevel <= 2)
                EnableButton();
            else
                DisableButton();
        }
    }

    public void CalculateCosts()
    {
        switch (unitToUpgrade)
        {
            case UnitToUpgrade.Pikeman:
                UpdateCostsVariables(_cost.goldUpgradePikeman,  _cost.woodUpgradePikeman, _cost.grainUpgradePikeman,
                                     _cost.sheepUpgradePikeman, _unitLevelController.pikemanLevel);
                break;
            case UnitToUpgrade.Archer:
                UpdateCostsVariables(_cost.goldUpgradeArcher,  _cost.woodUpgradeArcher, _cost.grainUpgradeArcher,
                                     _cost.sheepUpgradeArcher, _unitLevelController.archerLevel);
                break;
            case UnitToUpgrade.Footman:
                UpdateCostsVariables(_cost.goldUpgradeFootman,  _cost.woodUpgradeFootman, _cost.grainUpgradeFootman,
                                     _cost.sheepUpgradeFootman, _unitLevelController.footmanLevel);
                break;
            case UnitToUpgrade.Hussar:
                UpdateCostsVariables(_cost.goldUpgradeHussar,  _cost.woodUpgradeHussar, _cost.grainUpgradeHussar,
                                     _cost.sheepUpgradeHussar, _unitLevelController.hussarLevel);
                break;
            case UnitToUpgrade.Knight:
                UpdateCostsVariables(_cost.goldUpgradeKnight,  _cost.woodUpgradeKnight, _cost.grainUpgradeKnight,
                                     _cost.sheepUpgradeKnight, _unitLevelController.knightLevel);
                break;
            case UnitToUpgrade.ArmouredArcher:
                UpdateCostsVariables(_cost.goldUpgradeArmouredArcher, _cost.woodUpgradeArmouredArcher,
                                     _cost.grainUpgradeArmouredArcher,
                                     _cost.sheepUpgradeArmouredArcher, _unitLevelController.armouredArcherLevel);
                break;
            case UnitToUpgrade.MountedKnight:
                UpdateCostsVariables(_cost.goldUpgradeMountedKnight, _cost.woodUpgradeMountedKnight,
                                     _cost.grainUpgradeMountedKnight,
                                     _cost.sheepUpgradeMountedKnight, _unitLevelController.mountedKnightLevel);
                break;
        }
    }

    public void UpdateButtonState()
    {
        switch (unitToUpgrade)
        {
            case UnitToUpgrade.Pikeman:
                if (_unitLevelController.pikemanLevel         >= LevelController.Instance.maxUnitUpgradeLevel ||
                    LevelController.Instance.isPikemanEnabled == false)
                    DisableButton();
                break;
            case UnitToUpgrade.Archer:
                if (_unitLevelController.archerLevel >= LevelController.Instance.maxUnitUpgradeLevel ||
                    LevelController.Instance.isArcherEnabled == false)
                    DisableButton();
                break;
            case UnitToUpgrade.Footman:
                if (_unitLevelController.footmanLevel >= LevelController.Instance.maxUnitUpgradeLevel ||
                    LevelController.Instance.isFootmanEnabled == false)
                    DisableButton();
                break;
            case UnitToUpgrade.Hussar:
                if (_unitLevelController.hussarLevel >= LevelController.Instance.maxUnitUpgradeLevel ||
                    LevelController.Instance.isHussarEnabled == false)
                    DisableButton();
                break;
            case UnitToUpgrade.Knight:
                if (_unitLevelController.knightLevel >= LevelController.Instance.maxUnitUpgradeLevel ||
                    LevelController.Instance.isKnightEnabled == false)
                    DisableButton();
                break;
            case UnitToUpgrade.ArmouredArcher:
                if (_unitLevelController.armouredArcherLevel >= LevelController.Instance.maxUnitUpgradeLevel ||
                    LevelController.Instance.isArmouredArcherEnabled == false)
                    DisableButton();
                break;
            case UnitToUpgrade.MountedKnight:
                if (_unitLevelController.mountedKnightLevel >= LevelController.Instance.maxUnitUpgradeLevel ||
                    LevelController.Instance.isMountedKnightEnabled == false)
                    DisableButton();
                break;
        }

        if (_resourcesController.gold  >= _costGold  && _resourcesController.wood  >= _costWood &&
            _resourcesController.grain >= _costGrain && _resourcesController.sheep >= _costSheep)
        {
            if (_btnUpgradeUnit != null)
                _btnUpgradeUnit.interactable = true;
        }
        else
        {
            if (_btnUpgradeUnit != null)
                _btnUpgradeUnit.interactable = false;
        }
    }

    private void UpdateCostsVariables(int gold, int wood, int grain, int sheep, int level)
    {
        _costGold  = gold  * level;
        _costWood  = wood  * level;
        _costGrain = grain * level;
        _costSheep = sheep * level;
    }

    private void EnableButton()
    {
        _canvasGroup.alpha = 1f;
        _isDisabled        = false;
    }

    private void DisableButton()
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = 0f;
        
        _isDisabled        = true;
        _leanHover.ManualPointerExit();
    }

    public LeanHover GetLeanHover() { return _leanHover; }

    private void OnEnable()
    {
        CheckBuildingLevel();
        CalculateCosts();
        UpdateButtonState();
    }

    private void OnDisable() { _leanHover.ManualPointerExit(); }
}