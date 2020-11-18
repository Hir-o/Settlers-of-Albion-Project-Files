using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lean.Gui;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class ClickSettlementUpgradeButton : MonoBehaviour
{
    private static ClickSettlementUpgradeButton _instance;
    public static  ClickSettlementUpgradeButton Instance => _instance;

    [BoxGroup("Rank Images")]
    [SerializeField] private Image[] _ranks;

    [BoxGroup("Rank Colors")]
    [SerializeField] private Color _bronze, _silver, _gold;

    [BoxGroup("Upgrade Icons")]
    [SerializeField] private Sprite _sprVillageHouse, _sprCityHouse, _sprCastle;

    [BoxGroup("Settlement Building Icons / For Settlement Card Updating")]
    [SerializeField] private GameObject _iconHamlet, _iconVillage, _iconTown, _iconCity;

    [BoxGroup("Castle Building Icons / For Castle Card Updating")]
    [SerializeField] private GameObject _iconOutpost, _iconCastle;

    private Button              _btnUpgradeSettlement;
    private Image               _buttonImage;
    private CanvasGroup         _canvasGroup;
    private LeanHover           _leanHover;
    private ResourcesController _resourcesController;
    private Cost                _cost;
    private BuildingUIUpdater   _buildingUiUpdater;

    private int _costGold, _costWood, _costGrain, _costSheep, _costStone;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        _btnUpgradeSettlement = GetComponent<Button>();
        _buttonImage          = GetComponent<Image>();
        _canvasGroup          = GetComponentInParent<CanvasGroup>();
        _leanHover            = GetComponent<LeanHover>();
        _resourcesController  = ResourcesController.Instance;
        _cost                 = Cost.Instance;
        _buildingUiUpdater    = BuildingUIUpdater.Instance;

        _btnUpgradeSettlement.onClick.AddListener(UpgradeSettlementOnClick);
    }

    public void UpgradeSettlementOnClick()
    {
        if (_btnUpgradeSettlement.interactable == false) return;

        CalculateCosts();
        UpdateButtonState();
        
        _resourcesController.gold  -= _costGold;
        _resourcesController.wood  -= _costWood;
        _resourcesController.grain -= _costGrain;
        _resourcesController.sheep -= _costSheep;
        _resourcesController.stone -= _costStone;

        if (_buildingUiUpdater.selectedBuilding.isSettlement)
        {
            _buildingUiUpdater.selectedBuilding.Upgrade(_leanHover);

            if ((int) _buildingUiUpdater.selectedBuilding.upgradeLevel >=
                LevelController.Instance.maxSettlementUpgradeLevel)
            {
                _canvasGroup.alpha                 = 0f;
                _btnUpgradeSettlement.interactable = false;
            }
        }
        else if (_buildingUiUpdater.selectedBuilding.isStronghold)
        {
            _buildingUiUpdater.selectedBuilding.UpgradeStronghold(_leanHover);

            if ((int) _buildingUiUpdater.selectedBuilding.strongholdUpgradeLevel >=
                LevelController.Instance.maxStrongholdUpgradeLevel)
            {
                _canvasGroup.alpha                 = 0f;
                _btnUpgradeSettlement.interactable = false;
            }
        }

        _resourcesController.UpdateOnlyResourceUIText();

        UpdateRankColor();
        _buildingUiUpdater.UpdateStats();

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

        foreach (SpriteButton button in ObjectHolder.Instance.spriteButtons.Where(button => button != null))
            button.CalculateCosts();

        foreach (var button in ObjectHolder.Instance.marketBuyButtons) button.UpdateButtonState();

        foreach (var button in ObjectHolder.Instance.marketSellButtons) button.UpdateButtonState();

        CalculateCosts();
        UpdateButtonState();
        
        AudioController.Instance.SFXUpgradeBuilding();
    }

    public void CalculateCosts()
    {
        if (_buildingUiUpdater.selectedBuilding == null) return;

        switch (_buildingUiUpdater.selectedBuilding.upgradeLevel)
        {
            case Building.UpgradeLevel.Hamlet:
                UpdateCostsVariables(_cost.goldVillage, _cost.woodVillage, _cost.grainVillage, _cost.sheepVillage,
                                     _cost.stoneVillage);
                break;
            case Building.UpgradeLevel.Village:
                UpdateCostsVariables(_cost.goldTown, _cost.woodTown, _cost.grainTown, _cost.sheepTown,
                                     _cost.stoneTown);
                break;
            case Building.UpgradeLevel.Town:
                UpdateCostsVariables(_cost.goldCity, _cost.woodCity, _cost.grainCity, _cost.sheepCity,
                                     _cost.stoneCity);
                break;
        }

        switch (_buildingUiUpdater.selectedBuilding.strongholdUpgradeLevel)
        {
            case Building.StrongholdUpgradeLevel.Outpost:
                UpdateCostsVariables(_cost.goldCastle, _cost.woodCastle, _cost.grainCastle, _cost.sheepCastle,
                                     _cost.stoneCastle);
                break;
        }
    }

    public void UpdateButtonState()
    {
        if (_buildingUiUpdater.selectedBuilding == null) return;
        
        if ((int) _buildingUiUpdater.selectedBuilding.upgradeLevel >=
            LevelController.Instance.maxSettlementUpgradeLevel)
        {
            if (_btnUpgradeSettlement.interactable) _btnUpgradeSettlement.interactable = false;
            if (_canvasGroup.alpha > 0f) _canvasGroup.alpha = 0f;
            
            return;
        }
        
        if ((int) _buildingUiUpdater.selectedBuilding.strongholdUpgradeLevel >=
            LevelController.Instance.maxStrongholdUpgradeLevel)
        {
            if (_btnUpgradeSettlement.interactable) _btnUpgradeSettlement.interactable = false;
            if (_canvasGroup.alpha > 0f) _canvasGroup.alpha                            = 0f;
            
            return;
        }

        if (_resourcesController.gold  >= _costGold  && _resourcesController.wood  >= _costWood  &&
            _resourcesController.grain >= _costGrain && _resourcesController.sheep >= _costSheep &&
            _resourcesController.stone >= _costStone)
        {
            foreach (Image rank in _ranks) rank.DOFade(1f, 0f);

            _btnUpgradeSettlement.interactable = true;
        }
        else
        {
            foreach (Image rank in _ranks) rank.DOFade(.6f, 0f);

            _btnUpgradeSettlement.interactable = false;
        }
    }

    private void UpdateCostsVariables(int gold, int wood, int grain, int sheep, int stone)
    {
        _costGold  = gold;
        _costWood  = wood;
        _costGrain = grain;
        _costSheep = sheep;
        _costStone = stone;
    }

    private void UpdateRankColor()
    {
        _iconHamlet.SetActive(false);
        _iconVillage.SetActive(false);
        _iconTown.SetActive(false);
        _iconCity.SetActive(false);

        _iconOutpost.SetActive(false);
        _iconCastle.SetActive(false);

        if (BuildingUIUpdater.Instance.selectedBuilding.isSettlement)
        {
            switch (BuildingUIUpdater.Instance.selectedBuilding.upgradeLevel)
            {
                case Building.UpgradeLevel.Hamlet:
                    Array.ForEach(_ranks, x => x.color = _bronze);
                    _buttonImage.sprite = _sprVillageHouse;
                    if ((int) _buildingUiUpdater.selectedBuilding.upgradeLevel <
                        LevelController.Instance.maxSettlementUpgradeLevel)
                        _canvasGroup.alpha = 1f;
                    _iconHamlet.SetActive(true);
                    break;
                case Building.UpgradeLevel.Village:
                    Array.ForEach(_ranks, x => x.color = _silver);
                    _buttonImage.sprite = _sprCityHouse;
                    if ((int) _buildingUiUpdater.selectedBuilding.upgradeLevel <
                        LevelController.Instance.maxSettlementUpgradeLevel)
                        _canvasGroup.alpha = 1f;
                    _iconVillage.SetActive(true);
                    break;
                case Building.UpgradeLevel.Town:
                    Array.ForEach(_ranks, x => x.color = _gold);
                    _buttonImage.sprite = _sprCityHouse;
                    if ((int) _buildingUiUpdater.selectedBuilding.upgradeLevel <
                        LevelController.Instance.maxSettlementUpgradeLevel)
                        _canvasGroup.alpha = 1f;
                    _iconTown.SetActive(true);
                    break;
                case Building.UpgradeLevel.City:
                    _canvasGroup.alpha = 0f;
                    _iconCity.SetActive(true);
                    break;
            }
        }
        else if (BuildingUIUpdater.Instance.selectedBuilding.isStronghold)
        {
            switch (BuildingUIUpdater.Instance.selectedBuilding.strongholdUpgradeLevel)
            {
                case Building.StrongholdUpgradeLevel.Outpost:
                    Array.ForEach(_ranks, x => x.color = _gold);
                    _buttonImage.sprite = _sprCastle;
                    _canvasGroup.alpha  = 1f;
                    _iconOutpost.SetActive(true);
                    break;
                case Building.StrongholdUpgradeLevel.Castle:
                    _canvasGroup.alpha = 0f;
                    _iconCastle.SetActive(true);
                    break;
            }
        }
    }
    
    public LeanHover GetLeanHover() { return _leanHover; }

    private void OnEnable()
    {
        UpdateRankColor();

        CalculateCosts();
        UpdateButtonState();
    }
}