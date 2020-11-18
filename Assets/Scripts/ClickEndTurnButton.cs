using System.Collections;
using System.Collections.Generic;
using Lean.Gui;
using TbsFramework.Units;
using UnityEngine;
using UnityEngine.UI;

public class ClickEndTurnButton : MonoBehaviour
{
    [SerializeField] private Button _btnEndTurn, _btnNextUnit, _btnSkipUnit;

    [SerializeField] private Image _imgBg, _imgEndTurn, _imgNextUnit;

    private LeanHover _leanHover;

    private enum ListenerType
    {
        EndTurn,
        NextUnit,
        Skip
    }


    [SerializeField] private ListenerType _listenerType = ListenerType.EndTurn;

    private Color _bgColor;

    private void Awake()
    {
        _leanHover = GetComponent<LeanHover>();

        switch (_listenerType)
        {
            case ListenerType.EndTurn:
                _btnEndTurn = GetComponent<Button>();
                _btnEndTurn.onClick.AddListener(EndTurn);
                break;
            case ListenerType.NextUnit:
                _btnNextUnit = GetComponent<Button>();
                _btnNextUnit.onClick.AddListener(SelectNextUnit);
                break;
            case ListenerType.Skip:
                _btnSkipUnit = GetComponent<Button>();
                _btnSkipUnit.onClick.AddListener(SkipCurrentUnit);
                break;
        }
    }

    private void Start() { ObjectHolder.Instance.endTurnButtons.Add(this); }

    public void EndTurn()
    {
        _btnEndTurn.interactable  = false;
        _btnNextUnit.interactable = false;
        _btnSkipUnit.interactable = false;

        _bgColor   = _imgBg.color;
        _bgColor.a = .66f;

        _imgBg.color = _bgColor;

        if (UnitUIUpdater.Instance.panelUnit.gameObject.activeSelf)
            UnitUIUpdater.Instance.panelUnit.gameObject.SetActive(false);

        ObjectHolder.Instance.cellGrid.EndTurn();

        _leanHover.ManualPointerExit();
    }

    public void DisableButton()
    {
        _btnEndTurn.interactable  = false;
        _btnNextUnit.interactable = false;
        _btnSkipUnit.interactable = false;

        _bgColor   = _imgBg.color;
        _bgColor.a = .66f;

        _imgBg.color = _bgColor;

        if (UnitUIUpdater.Instance.panelUnit.gameObject.activeSelf)
            UnitUIUpdater.Instance.panelUnit.gameObject.SetActive(false);

        _leanHover.ManualPointerExit();
    }

    public void SelectNextUnit()
    {
        EndTurnUIUpdater.Instance.SelectNextIdleUnit();
        _leanHover.ManualPointerExit();
        
        AudioController.Instance.SFXButtonClick();
    }

    public void SkipCurrentUnit()
    {
//        EndTurnUIUpdater.Instance.SkipUnit();
        EndTurnUIUpdater.Instance.SkipSelectedUnit();
        //EndTurnUIUpdater.Instance.SelectNextIdleUnit();
        _leanHover.ManualPointerExit();
        
        AudioController.Instance.SFXButtonClick();
    }

    public void Reset()
    {
        _btnEndTurn.interactable  = true;
        _btnNextUnit.interactable = true;
        _btnSkipUnit.interactable = true;

        _bgColor   = _imgBg.color;
        _bgColor.a = 1f;

        _imgBg.color = _bgColor;
    }

    public void UpdateSkip(UnitController unit)
    {
        if (unit != null)
        {
            if (!unit.GetIsFinished() && unit.skipThisTurn == false && unit.unitType != Unit.UnitType.Building)
            {
                _btnSkipUnit.interactable = true;
                return;
            }
        }

        _btnSkipUnit.interactable = false;
    }

    private void OnDisable() { _leanHover.ManualPointerExit(); }
}