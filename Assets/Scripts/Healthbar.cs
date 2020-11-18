using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TbsFramework.Cells;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Transform _bar;

    [SerializeField] private SpriteRenderer _barSpriteRenderer,
                                            _healSpriteRenderer,
                                            _disbandSpriteRenderer,
                                            _mountainBonusSpriteRenderer,
                                            _underSiegeSpriteRenderer,
                                            _attritionSpriteRenderer,
                                            _noSuppliesSpriteRenderer;

    [SerializeField] private Color _normalColor, _warningColor, _dangerColor;

    [BoxGroup("Building Repair Hammer")]
    [SerializeField] private Sprite _spriteRepairHammer;

    private Vector3        _initBarScale;
    private UnitController _unitController;

    private void Awake()
    {
        _initBarScale = _bar.localScale;

        _unitController = GetComponentInParent<UnitController>();
        _unitController.SetHealthbar(this);
    }

    public void SetSize(float sizeNormalized)
    {
        if (DOTween.IsTweening(_bar, true)) DOTween.Kill(_bar);

        _bar.DOScaleY(sizeNormalized, sizeNormalized).SetEase(Ease.InCirc);

        UpdateColor(sizeNormalized);
    }

    public void UpdateColor(float sizeNormalized)
    {
        if (sizeNormalized >= .7f)
            _barSpriteRenderer.color = _normalColor;
        else if (sizeNormalized < .7f && sizeNormalized >= .4f)
            _barSpriteRenderer.color                            = _warningColor;
        else if (sizeNormalized < .4f) _barSpriteRenderer.color = _dangerColor;
    }

    public void UpdateHealIcon(bool value)       { _healSpriteRenderer.gameObject.SetActive(value); }
    public void UpdateDisbandIcon(bool value)    { _disbandSpriteRenderer.gameObject.SetActive(value); }
    public void UpdateUnderSiegeIcon(bool value) { _underSiegeSpriteRenderer.gameObject.SetActive(value); }

    public void UpdateMountainBonusIcon(HexagonTile cell)
    {
        _mountainBonusSpriteRenderer.gameObject.SetActive(cell.isMountain);
    }

    public void UpdateAttritionIcon(HexagonTile cell) { _attritionSpriteRenderer.gameObject.SetActive(cell.isHazard); }

    public void UpdateNoSuppliesIcon(bool hasSupplies)
    {
        if (ObjectHolder.Instance.cellGrid.CurrentPlayerNumber != 0) return;
        
        _noSuppliesSpriteRenderer.gameObject.SetActive(hasSupplies);
    }
    
    public void SetRepairHammerIcon() { _healSpriteRenderer.sprite = _spriteRepairHammer; }
}