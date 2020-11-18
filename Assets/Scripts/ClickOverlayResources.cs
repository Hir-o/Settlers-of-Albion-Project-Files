using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Gui;

public class ClickOverlayResources : MonoBehaviour
{
    private Button    _btnOverlayResources;
    private LeanHover _leanHover;

    private void Awake()
    {
        _leanHover           = GetComponent<LeanHover>();
        _btnOverlayResources = GetComponent<Button>();
        _btnOverlayResources.onClick.AddListener(OverlayResources);
    }

    public void OverlayResources()
    {
        foreach (HexagonTile tile in ObjectHolder.Instance.cellGrid.Cells) { tile.UpdateResourceIcon(); }

        _leanHover.ManualPointerExit();
        
        AudioController.Instance.SFXButtonClick();
    }
}