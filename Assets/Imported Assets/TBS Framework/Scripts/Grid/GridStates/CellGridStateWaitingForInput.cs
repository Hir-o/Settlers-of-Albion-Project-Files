using TbsFramework.Units;

namespace TbsFramework.Grid.GridStates
{
    class CellGridStateWaitingForInput : CellGridState
    {
        public CellGridStateWaitingForInput(CellGrid cellGrid) : base(cellGrid)
        {
        }

        public override void OnUnitClicked(Unit unit)
        {
            if (unit.PlayerNumber.Equals(_cellGrid.CurrentPlayerNumber))
            {
                if (unit.PlayerNumber == 0)
                    AudioController.Instance.SFXSelectUnit();
                
                _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, unit);
            }
        }
    }
}
