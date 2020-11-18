using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClickMarketButton : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransformMarket;

    private Button        _btnMarket;

    [SerializeField] private float _maxPosX = -3.7f, _minPosX = -507, _slideDuration = .25f;
    
    public bool isOpen;

    private void Awake()
    {
        _btnMarket = GetComponent<Button>();
        _btnMarket.onClick.AddListener(ToggleMarketPanel);

        ObjectHolder.Instance.marketButton = this;
    }

    public void ToggleMarketPanel()
    {
        if (ObjectHolder.Instance.cellGrid.CurrentPlayerNumber != 0) return;
        
        _btnMarket.interactable = false;

        if (isOpen)
        {
            _rectTransformMarket.DOAnchorPosX(_minPosX, _slideDuration).SetEase(Ease.InOutQuad).OnComplete(ResetButton);
            isOpen = false;
        }
        else
        {
            _rectTransformMarket.DOAnchorPosX(_maxPosX, _slideDuration).SetEase(Ease.InOutQuad).OnComplete(ResetButton);
            isOpen = true;
        }
        
        AudioController.Instance.SFXButtonClick();
    }

    private void ResetButton() { _btnMarket.interactable = true; }
}