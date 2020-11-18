using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TbsFramework.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUIUpdater : MonoBehaviour
{
    private static BuildingUIUpdater _instance;
    public static  BuildingUIUpdater Instance => _instance;

    [BoxGroup("UI Panels")]
    public GameObject panelBuilding;

    [BoxGroup("Colors")]
    [SerializeField] private Color _colorGreen, _colorRed;

    [BoxGroup("Building Name")]
    [SerializeField] private TextMeshProUGUI _tmpBuildingName;

    private readonly string _hamlet  = "Hamlet",
                            _village = "Village",
                            _town    = "Town",
                            _city    = "City",
                            _outpost = "Outpost",
                            _castle  = "Castle";

    [BoxGroup("Settlement Resource")]
    [SerializeField] private TextMeshProUGUI _tmpGold, _tmpWood, _tmpGrain, _tmpSheep, _tmpStone, _tmpHorse;

    [BoxGroup("Stats")]
    [SerializeField] private TextMeshProUGUI _tmpHealth, _tmpAttack, _tmpDefense, _tmpActionPoints;

    private bool _hasSettler,
                 _hasClubman,
                 _hasPikeman,
                 _hasArcher,
                 _hasFootman,
                 _hasHussar,
                 _hasWorker,
                 _hasKnight,
                 _hasArmouredArcher,
                 _hasMountedKnight,
                 _hasPaladin,
                 _hasMountedPaladin,
                 _hasKing;

    [BoxGroup("Unit Recruit Buttons")]
    [SerializeField] private GameObject _settler,
                                        _clubman,
                                        _pikeman,
                                        _archer,
                                        _footman,
                                        _hussar,
                                        _worker,
                                        _knight,
                                        _armouredArcher,
                                        _mountedKnight,
                                        _paladin,
                                        _mountedPaladin,
                                        _king;

    [BoxGroup("Unit Upgrade Buttons")]
    [SerializeField] private GameObject _pikemanUpgrade,
                                        _archerUpgrade,
                                        _footmanUpgrade,
                                        _hussarUpgrade,
                                        _knightUpgrade,
                                        _armouredArcherUpgrade,
                                        _mountedKnightUpgrade;

    [BoxGroup("Barracks Rect Transform")]
    [SerializeField] private RectTransform _barracks;

    [BoxGroup("Barracks Width Sizes")]
    [SerializeField] private float _barracksHamletWidth  = 216f,
                                   _barracksVillageWidth = 321f,
                                   _barracksTownWidth    = 434f,
                                   _barracksCityWidth    = 650f;

    [BoxGroup("Barrack / Panel Upgrades Layout Group")]
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;

    [BoxGroup("Barrack / Panel Upgrades Layout Group")]
    [SerializeField] private int _settlementPaddingLeft = 109, _castlePaddingLeft = 0;

    [HideInInspector]
    public Building selectedBuilding;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        panelBuilding.SetActive(false);
    }

    public void UpdateBuildingPanel(Building building)
    {
        selectedBuilding = building;

        if (UnitUIUpdater.Instance.panelUnit.activeSelf) UnitUIUpdater.Instance.panelUnit.SetActive(false);

        panelBuilding.SetActive(true);

        UpdateResources();
        UpdateBuildingButtons();
        CheckForBuildingButtons();
    }

    public void UpdateResources()
    {
        if (selectedBuilding == null) return;

        UpdateBuildingName();
        UpdateStats();

        UpdateResource(_tmpGold,  selectedBuilding.gold,  true,  true);
        UpdateResource(_tmpWood,  selectedBuilding.wood,  true,  false);
        UpdateResource(_tmpGrain, selectedBuilding.grain, true,  false);
        UpdateResource(_tmpSheep, selectedBuilding.sheep, true,  false);
        UpdateResource(_tmpStone, selectedBuilding.stone, true,  false);
        UpdateResource(_tmpHorse, selectedBuilding.horse, false, false);
    }

    private void UpdateBuildingName()
    {
        if (selectedBuilding.isSettlement)
        {
            switch (selectedBuilding.upgradeLevel)
            {
                case Building.UpgradeLevel.Hamlet:
                    _tmpBuildingName.text = _hamlet;
                    break;
                case Building.UpgradeLevel.Village:
                    _tmpBuildingName.text = _village;
                    break;
                case Building.UpgradeLevel.Town:
                    _tmpBuildingName.text = _town;
                    break;
                case Building.UpgradeLevel.City:
                    _tmpBuildingName.text = _city;
                    break;
            }
        }
        else if (selectedBuilding.isStronghold)
        {
            switch (selectedBuilding.strongholdUpgradeLevel)
            {
                case Building.StrongholdUpgradeLevel.Outpost:
                    _tmpBuildingName.text = _outpost;
                    break;
                case Building.StrongholdUpgradeLevel.Castle:
                    _tmpBuildingName.text = _castle;
                    break;
            }
        }
    }

    private void UpdateResource(TextMeshProUGUI _tmpResource, int resourceVal, bool addSign, bool isGold)
    {
        if (selectedBuilding.isSettlement)
        {
            switch (selectedBuilding.upgradeLevel)
            {
                case Building.UpgradeLevel.Hamlet:
                    resourceVal *= 1;
                    break;
                case Building.UpgradeLevel.Village:
                    resourceVal *= 2;
                    break;
                case Building.UpgradeLevel.Town:
                    resourceVal *= 3;
                    break;
                case Building.UpgradeLevel.City:
                    resourceVal *= 4;
                    break;
            }

            if (isGold)
            {
                for (int i = (int) selectedBuilding.upgradeLevel - 1; i > 0; i--)
                    resourceVal += i;
            }
        }
        else if (selectedBuilding.isStronghold)
        {
            switch (selectedBuilding.strongholdUpgradeLevel)
            {
                case Building.StrongholdUpgradeLevel.Castle:
                    //resourceVal += resourceVal;
                    break;
            }
        }

        if (resourceVal == 0 || addSign == false)
        {
            _tmpResource.text  = resourceVal.ToString();
            _tmpResource.color = _colorGreen;
        }
        else if (resourceVal > 0)
        {
            _tmpResource.text  = "+" + resourceVal;
            _tmpResource.color = _colorGreen;
        }
        else if (resourceVal < 0)
        {
            _tmpResource.text  = resourceVal.ToString();
            _tmpResource.color = _colorRed;
        }
    }

    public void UpdateStats()
    {
        _tmpHealth.text       = selectedBuilding.GetUnitController().HitPoints.ToString();
        _tmpAttack.text       = selectedBuilding.GetUnitController().AttackFactor.ToString();
        _tmpDefense.text      = selectedBuilding.GetUnitController().DefenceFactor.ToString();
        _tmpActionPoints.text = selectedBuilding.GetUnitController().ActionPoints.ToString();
    }

    public void CheckForBuildingButtons()
    {
        _hasSettler = false;
        _hasClubman = false;
        _hasPikeman = false;
        _hasArcher  = false;
        _hasFootman = false;
        _hasHussar  = false;
        _hasWorker  = false;

        _hasKnight         = false;
        _hasArmouredArcher = false;
        _hasMountedKnight  = false;
        _hasPaladin        = false;
        _hasMountedPaladin = false;
        _hasKing           = false;

        if (selectedBuilding.isSettlement)
        {
            _horizontalLayoutGroup.padding.left = _settlementPaddingLeft;
            switch (selectedBuilding.upgradeLevel)
            {
                case Building.UpgradeLevel.Hamlet:
                    _hasSettler = true;
                    _hasClubman = true;
                    _barracks.DOSizeDelta(new Vector2(_barracksHamletWidth, _barracks.rect.height), 0f);
                    break;
                case Building.UpgradeLevel.Village:
                    _hasSettler = true;
                    _hasPikeman = true;
                    _hasArcher  = true;
                    _barracks.DOSizeDelta(new Vector2(_barracksVillageWidth, _barracks.rect.height), 0f);
                    break;
                case Building.UpgradeLevel.Town:
                    _hasSettler = true;
                    _hasPikeman = true;
                    _hasArcher  = true;
                    _hasFootman = true;
                    _barracks.DOSizeDelta(new Vector2(_barracksTownWidth, _barracks.rect.height), 0f);
                    break;
                case Building.UpgradeLevel.City:
                    _hasSettler = true;
                    _hasPikeman = true;
                    _hasArcher  = true;
                    _hasFootman = true;
                    _hasHussar  = true;
                    _hasWorker  = true;
                    _barracks.DOSizeDelta(new Vector2(_barracksCityWidth, _barracks.rect.height), 0f);
                    break;
            }
        }
        else if (selectedBuilding.isStronghold)
        {
            _horizontalLayoutGroup.padding.left = _castlePaddingLeft;
            switch (selectedBuilding.strongholdUpgradeLevel)
            {
                case Building.StrongholdUpgradeLevel.Outpost:
                    _hasKnight         = true;
                    _hasArmouredArcher = true;
                    _hasMountedKnight  = true;
                    _barracks.DOSizeDelta(new Vector2(_barracksVillageWidth, _barracks.rect.height), 0f);
                    break;
                case Building.StrongholdUpgradeLevel.Castle:
                    _hasKnight         = true;
                    _hasArmouredArcher = true;
                    _hasMountedKnight  = true;
                    _hasPaladin        = true;
                    _hasMountedPaladin = true;
                    _hasKing           = true;
                    _barracks.DOSizeDelta(new Vector2(_barracksCityWidth, _barracks.rect.height), 0f);
                    break;
            }
        }

        UpdateBuildingButtons();
    }

    private void UpdateBuildingButtons()
    {
        ShowButton(_settler,        _hasSettler);
        ShowButton(_clubman,        _hasClubman);
        ShowButton(_worker,         _hasWorker);
        ShowButton(_paladin,        _hasPaladin);
        ShowButton(_mountedPaladin, _hasMountedPaladin);
        ShowButton(_king,           _hasKing);

        ShowButton(_pikeman,        _hasPikeman,        _pikemanUpgrade);
        ShowButton(_archer,         _hasArcher,         _archerUpgrade);
        ShowButton(_footman,        _hasFootman,        _footmanUpgrade);
        ShowButton(_hussar,         _hasHussar,         _hussarUpgrade);
        ShowButton(_knight,         _hasKnight,         _knightUpgrade);
        ShowButton(_armouredArcher, _hasArmouredArcher, _armouredArcherUpgrade);
        ShowButton(_mountedKnight,  _hasMountedKnight,  _mountedKnightUpgrade);
    }

    /// <summary>
    ///For units that don't have upgrades
    /// </summary>
    private void ShowButton(GameObject unit, bool isEnabled)
    {
        if (isEnabled)
            unit.SetActive(true);
        else
            unit.SetActive(false);
    }

    /// <summary>
    ///For units that have upgrades
    /// </summary>
    private void ShowButton(GameObject unit, bool isEnabled, GameObject upgradeButton)
    {
        if (isEnabled)
        {
            unit.SetActive(true);
            upgradeButton.SetActive(true);
        }
        else
        {
            unit.SetActive(false);
            upgradeButton.SetActive(false);
        }
    }
}