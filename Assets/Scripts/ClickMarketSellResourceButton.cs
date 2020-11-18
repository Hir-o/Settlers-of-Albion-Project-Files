using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickMarketSellResourceButton : MonoBehaviour
{
    private Button _btnSell;

    private ResourcesController _resourcesController;
    private Cost                _cost;

    [SerializeField] private int _resourceSellAmount = 10;

    [BoxGroup("TMP Prices")]
    [SerializeField] private TextMeshProUGUI _priceText;

    public enum SellResourceType
    {
        Wood,
        Grain,
        Sheep,
        Stone
    }

    public SellResourceType resourceType = SellResourceType.Wood;

    private void Awake()
    {
        _resourcesController = ResourcesController.Instance;
        _cost                = Cost.Instance;
        _btnSell             = GetComponent<Button>();
        _btnSell.onClick.AddListener(SellOnClick);

        ObjectHolder.Instance.marketSellButtons.Add(this);

        UpdateButtonState();
    }

    public void SellOnClick()
    {
        if (ObjectHolder.Instance.cellGrid.CurrentPlayerNumber != 0) return;

        switch (resourceType)
        {
            case SellResourceType.Wood:
                SellResource(_cost.sellWood);
                break;
            case SellResourceType.Grain:
                SellResource(_cost.sellGrain);
                break;
            case SellResourceType.Sheep:
                SellResource(_cost.sellSheep);
                break;
            case SellResourceType.Stone:
                SellResource(_cost.sellStone);
                break;
        }

        _resourcesController.UpdateOnlyResourceUIText();
        ObjectHolder.Instance.UpdateAllButtonStates();
    }

    private void SellResource(int sellPrice)
    {
        int   multipliedPrice = sellPrice * _cost.marketPricesMultiplier;
        float finalPrice      = multipliedPrice + multipliedPrice * ((float) _cost.marketFeeReduceFactor * 10 / 100);

        switch (resourceType)
        {
            case SellResourceType.Wood:
                if (_resourcesController.wood >= _resourceSellAmount)
                {
                    _resourcesController.wood -= _resourceSellAmount;
                    _resourcesController.gold += Mathf.RoundToInt(finalPrice);
                    _btnSell.interactable     =  true;
                    AudioController.Instance.SFXMarketSell();
                    return;
                }

                break;
            case SellResourceType.Grain:
                if (_resourcesController.grain >= _resourceSellAmount)
                {
                    _resourcesController.grain -= _resourceSellAmount;
                    _resourcesController.gold  += Mathf.RoundToInt(finalPrice);
                    _btnSell.interactable      =  true;
                    AudioController.Instance.SFXMarketSell();
                    return;
                }

                break;
            case SellResourceType.Sheep:
                if (_resourcesController.sheep >= _resourceSellAmount)
                {
                    _resourcesController.sheep -= _resourceSellAmount;
                    _resourcesController.gold  += Mathf.RoundToInt(finalPrice);
                    _btnSell.interactable      =  true;
                    AudioController.Instance.SFXMarketSell();
                    return;
                }

                break;
            case SellResourceType.Stone:
                if (_resourcesController.stone >= _resourceSellAmount)
                {
                    _resourcesController.stone -= _resourceSellAmount;
                    _resourcesController.gold  += Mathf.RoundToInt(finalPrice);
                    _btnSell.interactable      =  true;
                    AudioController.Instance.SFXMarketSell();
                    return;
                }

                break;
        }

        _btnSell.interactable = false;
    }

    public void UpdateButtonState()
    {
        int price = 0;

        switch (resourceType)
        {
            case SellResourceType.Wood:
                CheckCosts();
                price = _cost.sellWood;
                break;
            case SellResourceType.Grain:
                CheckCosts();
                price = _cost.sellGrain;
                break;
            case SellResourceType.Sheep:
                CheckCosts();
                price = _cost.sellSheep;
                break;
            case SellResourceType.Stone:
                CheckCosts();
                price = _cost.sellStone;
                break;
        }

        float multipliedPrice = price * _cost.marketPricesMultiplier;
        float finalPrice = multipliedPrice + multipliedPrice *
                           ((float) _cost.marketFeeReduceFactor * 10 / 100);

        _priceText.text = $"<sprite=0 index=4>{Mathf.RoundToInt(finalPrice)}";
    }

    private void CheckCosts()
    {
        switch (resourceType)
        {
            case SellResourceType.Wood:
                if (_resourcesController.wood >= _resourceSellAmount)
                    _btnSell.interactable = true;
                else
                    _btnSell.interactable = false;

                break;
            case SellResourceType.Grain:
                if (_resourcesController.grain >= _resourceSellAmount)
                    _btnSell.interactable = true;
                else
                    _btnSell.interactable = false;

                break;
            case SellResourceType.Sheep:
                if (_resourcesController.sheep >= _resourceSellAmount)
                    _btnSell.interactable = true;
                else
                    _btnSell.interactable = false;

                break;
            case SellResourceType.Stone:
                if (_resourcesController.stone >= _resourceSellAmount)
                    _btnSell.interactable = true;
                else
                    _btnSell.interactable = false;

                break;
        }
    }
}