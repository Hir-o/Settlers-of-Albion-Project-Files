using System.Collections;
using System.Collections.Generic;
using TbsFramework.Cells;
using TbsFramework.Grid.GridStates;
using TbsFramework.Units;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    private static UnitSpawner _instance;
    public static  UnitSpawner Instance => _instance;

    private Transform _unitsParent;

    public readonly static string UnitsGameObjectName = "Units";

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        _unitsParent = GameObject.Find(UnitsGameObjectName).transform;
    }

    public void SpawnUnit(UnitController unitToSpawn, Cell cell)
    {
        ResourcesController.Instance.gold  -= Cost.Instance.unitToSpawnGold;
        ResourcesController.Instance.wood  -= Cost.Instance.unitToSpawnWood;
        ResourcesController.Instance.grain -= Cost.Instance.unitToSpawnGrain;
        ResourcesController.Instance.sheep -= Cost.Instance.unitToSpawnSheep;
        ResourcesController.Instance.stone -= Cost.Instance.unitToSpawnStone;
        ResourcesController.Instance.horse -= Cost.Instance.unitToSpawnHorse;
        
        var cellGrid = ObjectHolder.Instance.cellGrid;

        UnitController unit = Instantiate(unitToSpawn, cell.transform.position, Quaternion.Euler(0f, 0f, -90f))
            .GetComponent<UnitController>();

        unit.Cell             = cell;
        unit.Cell.CurrentUnit = unit;
        unit.Initialize();
        unit.transform.SetParent(_unitsParent);

        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
        cellGrid.AddUnit(unit.GetComponent<Transform>());

        cell.IsTaken = true;

        EndTurnUIUpdater.Instance.UpdateEndTurnButton();

        ResetHighlightedTiles();

        foreach (ClickUnitButton button in ObjectHolder.Instance.unitButtons)
        {
            button.CalculateCosts();
            button.UpdateButtonState();
        }

        foreach (var button in ObjectHolder.Instance.marketBuyButtons) button.UpdateButtonState();

        foreach (var button in ObjectHolder.Instance.marketSellButtons) button.UpdateButtonState();

        ResourcesController.Instance.UpdateOnlyResourceUIText();

        StartCoroutine(ClickUnit(unit));

        AudioController.Instance.SFXRecruitUnit();
    }
    
    public void SpawnUnitSpecial(UnitController unitToSpawn, Cell cell)
    {
        var cellGrid = ObjectHolder.Instance.cellGrid;

        UnitController unit = Instantiate(unitToSpawn, cell.transform.position, Quaternion.Euler(0f, 0f, -90f))
            .GetComponent<UnitController>();

        unit.Cell             = cell;
        unit.Cell.CurrentUnit = unit;
        unit.Initialize();
        unit.transform.SetParent(_unitsParent);

        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
        cellGrid.AddUnit(unit.GetComponent<Transform>());

        cell.IsTaken = true;

        EndTurnUIUpdater.Instance.UpdateEndTurnButton();

        ResetHighlightedTiles();

        ResourcesController.Instance.UpdateOnlyResourceUIText();
    }
    
    private IEnumerator ClickUnit(UnitController unit)
    {
        yield return new WaitForSeconds(.001f);
        ObjectHolder.Instance.cellGrid.CellGridState.OnUnitClicked(unit);
        ObjectHolder.Instance.cellGrid.CellGridState =
            new CellGridStateUnitSelected(ObjectHolder.Instance.cellGrid, unit.GetComponent<Unit>());
    }

    public UnitController SpawnEnemyUnit(UnitController unitToSpawn, Cell cell, int level)
    {
        var cellGrid = ObjectHolder.Instance.cellGrid;

        UnitController unit = Instantiate(unitToSpawn, cell.transform.position, Quaternion.Euler(0f, 0f, -90f))
            .GetComponent<UnitController>();

        unit.Cell             = cell;
        unit.Cell.CurrentUnit = unit;
        unit.Initialize();
        unit.transform.SetParent(_unitsParent);

        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
        cellGrid.AddUnit(unit.GetComponent<Transform>());

        cell.IsTaken = true;

        unit._level = (UnitController.Level) level;

        unit.UpdateUnitLevel();
        ResetHighlightedTiles();

        return unit;
    }

    public void ResetHighlightedTiles()
    {
        if (ObjectHolder.Instance.purchasedUnitGhost != null) Destroy(ObjectHolder.Instance.purchasedUnitGhost);

        foreach (Building settlement in ObjectHolder.Instance.buildings) { settlement.ResetHighlightedTile(); }
    }
}