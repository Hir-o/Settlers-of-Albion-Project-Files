using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;

public class ClickDisbandButton : MonoBehaviour
{
    private Button    _btnDisband;
    private LeanHover _leanHover;

    private void Awake()
    {
        _btnDisband = GetComponent<Button>();
        _leanHover  = GetComponent<LeanHover>();
        _btnDisband.onClick.AddListener(DisbandOnClick);
        
        ObjectHolder.Instance.disbandButtons.Add(this);
    }

    public void DisbandOnClick()
    {
        UnitUIUpdater.Instance.selectedUnit.Disband();
        _leanHover.ManualPointerExit();
        
        AudioController.Instance.SFXButtonClick();
    }

    public LeanHover GetLeanHover() { return _leanHover; }
}