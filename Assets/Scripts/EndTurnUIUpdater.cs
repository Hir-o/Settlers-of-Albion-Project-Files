using System;
using System.Collections;
using System.Collections.Generic;
using TbsFramework.Cells;
using TbsFramework.Grid;
using TbsFramework.Grid.GridStates;
using TbsFramework.Units;
using TbsFramework.Units.UnitStates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnUIUpdater : MonoBehaviour
{
    private static EndTurnUIUpdater _instance;
    public static  EndTurnUIUpdater Instance => _instance;

    [SerializeField] private ClickEndTurnButton _btnEndTurn, _btnNextUnit, _btnSkipUnit;

    [SerializeField] private TextMeshProUGUI _turnCounter;

    private UnitController _unitController;

    [HideInInspector]
    public UnitController currentSelectedUnit;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateEndTurnButton();
        UpdateSkipButton();
        UpdateTurnCounter();
    }

    public void UpdateEndTurnButton()
    {
        foreach (Unit unit in ObjectHolder.Instance.cellGrid.Units)
        {
            if (unit != null && unit.PlayerNumber == 0)
            {
                _unitController = (UnitController) unit;

                if (!_unitController.GetIsFinished() && _unitController.skipThisTurn == false                  &&
                    _unitController.unitType                                         != Unit.UnitType.Building &&
                    _unitController.GetIsDisbanding()                                == false)
                {
                    _btnEndTurn.gameObject.SetActive(false);
                    _btnNextUnit.gameObject.SetActive(true);

                    return;
                }
            }
        }

        _btnEndTurn.gameObject.SetActive(true);
        _btnNextUnit.gameObject.SetActive(false);
    }

    public void SelectNextIdleUnit()
    {
        foreach (Unit unit in ObjectHolder.Instance.cellGrid.Units)
        {
            if (unit != null && unit.PlayerNumber == 0)
            {
                _unitController = (UnitController) unit;

                if (!_unitController.GetIsFinished() && _unitController.skipThisTurn == false &&
                    _unitController.unitType                                         != Unit.UnitType.Building)
                {
                    ObjectHolder.Instance.cellGrid.CellGridState.OnUnitClicked(_unitController);
                    ObjectHolder.Instance.cellGrid.CellGridState =
                        new CellGridStateUnitSelected(ObjectHolder.Instance.cellGrid, unit);

                    TBSCamera.Instance.FocusOn(_unitController.transform);
                    return;
                }
            }
        }

        ObjectHolder.Instance.cellGrid.CellGridState = new CellGridStateWaitingForInput(ObjectHolder.Instance.cellGrid);
        _btnEndTurn.gameObject.SetActive(true);
        _btnNextUnit.gameObject.SetActive(false);
    }

    public void SkipUnit()
    {
        foreach (Unit unit in ObjectHolder.Instance.cellGrid.Units)
        {
            if (unit.PlayerNumber == 0)
            {
                _unitController = (UnitController) unit;

                if (!_unitController.GetIsFinished() && _unitController.skipThisTurn == false)
                {
                    _unitController.skipThisTurn = true;
                    _unitController.OnUnitDeselected();
                    return;
                }
            }
        }
    }
    
    public void SkipSelectedUnit(UnitController selectedUnit)
    {
        if (!selectedUnit.GetIsFinished() && selectedUnit.skipThisTurn == false)
        {
            selectedUnit.skipThisTurn = true;
            selectedUnit.OnUnitDeselected();
        }
    }

    public void SkipSelectedUnit()
    {
        if (currentSelectedUnit != null)
        {
            currentSelectedUnit.skipThisTurn = true;
            UpdateSkipButton();
            UpdateEndTurnButton();
        }
    }

    public void UpdateSkipButton() { _btnSkipUnit.UpdateSkip(currentSelectedUnit); }

    public void UpdateTurnCounter() { _turnCounter.text = ObjectHolder.Instance.cellGrid.turnCount.ToString(); }

    public void ResetButtons() { _btnEndTurn.Reset(); }
}