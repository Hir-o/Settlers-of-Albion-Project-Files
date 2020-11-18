using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;

public class ClickFoundCityButton : MonoBehaviour
{
    private Button _btnFoundCity;
    private LeanHover _leanHover;

    private void Awake()
    {
        _btnFoundCity = GetComponent<Button>();
        _leanHover = GetComponent<LeanHover>();
        _btnFoundCity.onClick.AddListener(SettleOnClick);
        ObjectHolder.Instance.foundCityButton = this;
    }

    private void SettleOnClick()
    {
        HexagonTile tile = (HexagonTile) UnitUIUpdater.Instance.selectedUnit.Cell;

        if (tile.isHazard == false)
        {
            UnitUIUpdater.Instance.selectedUnit.FormSettlement(false);
            _leanHover.ManualPointerExit();
        }
    }

    public LeanHover GetLeanHover() { return _leanHover; }
}
