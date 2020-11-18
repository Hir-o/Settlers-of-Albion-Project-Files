using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;

public class ClickHealButton : MonoBehaviour
{
    private Button _btnHeal;
    private LeanHover _leanHover;

    private void Awake()
    {
        _btnHeal = GetComponent<Button>();
        _leanHover = GetComponent<LeanHover>();
        _btnHeal.onClick.AddListener(HealOnClick);
        ObjectHolder.Instance.healButton = this;
    }

    public void HealOnClick()
    {
        HexagonTile tile = (HexagonTile) UnitUIUpdater.Instance.selectedUnit.Cell;
        
        if (tile.isHazard == false && UnitUIUpdater.Instance.selectedUnit.ActionPoints > 0 && UnitUIUpdater.Instance.selectedUnit.GetHasSupplies())
            UnitUIUpdater.Instance.selectedUnit.SetIsHealing(true);
        
        AudioController.Instance.SFXButtonClick();
    }
    
    public LeanHover GetLeanHover() { return _leanHover; }
}