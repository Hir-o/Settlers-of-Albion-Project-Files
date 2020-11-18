using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickMarketBuyResourceButton : MonoBehaviour
{
    private Button _btnBuy;

    private ResourcesController _resourcesController;
    private Cost                _cost;

    [SerializeField] private int _resourceBuyAmount = 10;

    [BoxGroup("TMP Prices")]
    [SerializeField] private TextMeshProUGUI _priceText;

    public enum BuyResourceType
    {
        Wood,
        Grain,
        Sheep,
        Stone
    }

    public BuyResourceType resourceType = BuyResourceType.Wood;

    private void Awake()
    {
        _resourcesController = ResourcesController.Instance;
        _cost                = Cost.Instance;
        _btnBuy              = GetComponent<Button>();
        _btnBuy.onClick.AddListener(BuyOnClick);

        ObjectHolder.Instance.marketBuyButtons.Add(this);

        UpdateButtonState();
    }

    public void BuyOnClick()
    {
        if (ObjectHolder.Instance.cellGrid.CurrentPlayerNumber != 0) return;

        switch (resourceType)
        {
            case BuyResourceType.Wood:
                if (HasEnoughGold(_cost.buyWood))
                {
                    _resourcesController.wood += _resourceBuyAmount * _cost.marketPricesMultiplier;
                    AudioController.Instance.SFXMarketBuy();
                }

                break;
            case BuyResourceType.Grain:
                if (HasEnoughGold(_cost.buyGrain))
                {
                    _resourcesController.grain += _resourceBuyAmount * _cost.marketPricesMultiplier;
                    AudioController.Instance.SFXMarketBuy();
                }

                break;
            case BuyResourceType.Sheep:
                if (HasEnoughGold(_cost.buySheep))
                {
                    _resourcesController.sheep += _resourceBuyAmount * _cost.marketPricesMultiplier;
                    AudioController.Instance.SFXMarketBuy();
                }

                break;
            case BuyResourceType.Stone:
                if (HasEnoughGold(_cost.buyStone))
                {
                    _resourcesController.stone += _resourceBuyAmount * _cost.marketPricesMultiplier;
                    AudioController.Instance.SFXMarketBuy();
                }

                break;
        }

        foreach (UnitController unit in ObjectHolder.Instance.cellGrid.Units)
        {
            if (unit.PlayerNumber == 0 && unit.GetHasSupplies() == false) unit.CheckForSupplies(false);
        }

        if (Quests.Instance != null) { Quests.Instance.CheckOptionalQuests(); }

        _resourcesController.UpdateOnlyResourceUIText();
        ObjectHolder.Instance.UpdateAllButtonStates();
    }

    private bool HasEnoughGold(int price)
    {
        int   multipliedPrice = price * _cost.marketPricesMultiplier;
        float finalPrice      = multipliedPrice - multipliedPrice * ((float) _cost.marketFeeReduceFactor * 10 / 100);

        if (_resourcesController.gold >= Mathf.RoundToInt(finalPrice))
        {
            _resourcesController.gold -= Mathf.RoundToInt(finalPrice);
            _btnBuy.interactable      =  true;

            return true;
        }

        _btnBuy.interactable = false;
        return false;
    }

    public void UpdateButtonState()
    {
        int price = 0;

        switch (resourceType)
        {
            case BuyResourceType.Wood:
                CheckCosts(_cost.buyWood);
                price = _cost.buyWood;
                break;
            case BuyResourceType.Grain:
                CheckCosts(_cost.buyGrain);
                price = _cost.buyGrain;
                break;
            case BuyResourceType.Sheep:
                CheckCosts(_cost.buySheep);
                price = _cost.buySheep;
                break;
            case BuyResourceType.Stone:
                CheckCosts(_cost.buyStone);
                price = _cost.buyStone;
                break;
        }

        float multipliedPrice = price * _cost.marketPricesMultiplier;
        float finalPrice = multipliedPrice - multipliedPrice *
                           ((float) _cost.marketFeeReduceFactor * 10 / 100);

        _priceText.text = $"<sprite=0 index=4>{Mathf.RoundToInt(finalPrice)}";
    }

    private void CheckCosts(int price)
    {
        int   multipliedPrice = price * _cost.marketPricesMultiplier;
        float finalPrice      = multipliedPrice - multipliedPrice * ((float) _cost.marketFeeReduceFactor * 10 / 100);

        if (_resourcesController.gold >= Mathf.RoundToInt(finalPrice))
        {
            if (_btnBuy != null) _btnBuy.interactable = true;
        }
        else
        {
            if (_btnBuy != null) _btnBuy.interactable = false;
        }
    }
}