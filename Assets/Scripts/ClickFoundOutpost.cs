using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;

public class ClickFoundOutpost : MonoBehaviour
{
    private Button _btnFoundOupost;
    private LeanHover _leanHover;

    private void Awake()
    {
        _btnFoundOupost = GetComponent<Button>();
        _leanHover = GetComponent<LeanHover>();
        _btnFoundOupost.onClick.AddListener(OutpostOnClick);
        ObjectHolder.Instance.foundOutpostButton = this;
    }

    private void OutpostOnClick()
    {
        HexagonTile tile = (HexagonTile) UnitUIUpdater.Instance.selectedUnit.Cell;

        if (tile.isHazard == false)
        {
            UnitUIUpdater.Instance.selectedUnit.FormSettlement(true);
            _leanHover.ManualPointerExit();
        }
    }

    public LeanHover GetLeanHover() { return _leanHover; }
}
