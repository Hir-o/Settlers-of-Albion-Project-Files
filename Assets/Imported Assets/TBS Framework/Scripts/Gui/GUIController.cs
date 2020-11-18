using System;
using TbsFramework.Grid;
using TbsFramework.Grid.GridStates;
using UnityEngine;

namespace TbsFramework.Gui
{
    public class GUIController : MonoBehaviour
    {
        public CellGrid CellGrid;

        void Awake()
        {
            CellGrid.LevelLoading     += onLevelLoading;
            CellGrid.LevelLoadingDone += onLevelLoadingDone;
        }

        private void onLevelLoading(object sender, EventArgs e)
        {
//            Debug.Log("Level is loading");
        }

        private void onLevelLoadingDone(object sender, EventArgs e)
        {
//            Debug.Log("Level loading done");
//            Debug.Log("Press 'n' to end turn");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N) && ObjectHolder.Instance.cellGrid.CurrentPlayerNumber == 0)
            {
                foreach (ClickUpgradeUnitButton button in ObjectHolder.Instance.upgradeUnitButtons)
                    button.GetLeanHover().ManualPointerExit();

                foreach (ClickUnitButton button in ObjectHolder.Instance.unitButtons)
                    button.GetLeanHover().ManualPointerExit();

                if (EndTurnUIUpdater.Instance.currentSelectedUnit != null)
                    EndTurnUIUpdater.Instance.SkipSelectedUnit(EndTurnUIUpdater.Instance.currentSelectedUnit);

                EndTurnUIUpdater.Instance.SelectNextIdleUnit();

                if (ObjectHolder.Instance.foundCityButton != null)
                    ObjectHolder.Instance.foundCityButton.GetLeanHover().ManualPointerExit();

                if (ObjectHolder.Instance.foundOutpostButton != null)
                    ObjectHolder.Instance.foundOutpostButton.GetLeanHover().ManualPointerExit();

                if (ObjectHolder.Instance.healButton != null)
                    ObjectHolder.Instance.healButton.GetLeanHover().ManualPointerExit();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                foreach (HexagonTile tile in ObjectHolder.Instance.cellGrid.Cells) { tile.UpdateResourceIcon(); }
            }

            if (Input.GetKeyDown(KeyCode.U) && ObjectHolder.Instance.cellGrid.CurrentPlayerNumber == 0)
            {
                if (ObjectHolder.Instance.foundCityButton != null)
                    ObjectHolder.Instance.foundCityButton.GetLeanHover().ManualPointerExit();

                if (ObjectHolder.Instance.foundOutpostButton != null)
                    ObjectHolder.Instance.foundOutpostButton.GetLeanHover().ManualPointerExit();

                if (ObjectHolder.Instance.healButton != null)
                    ObjectHolder.Instance.healButton.GetLeanHover().ManualPointerExit();

                foreach (var button in ObjectHolder.Instance.upgradeUnitButtons)
                    button.GetLeanHover().ManualPointerExit();

                foreach (var button in ObjectHolder.Instance.unitButtons) button.GetLeanHover().ManualPointerExit();

                foreach (var button in ObjectHolder.Instance.disbandButtons) button.GetLeanHover().ManualPointerExit();

                if (ClickSettlementUpgradeButton.Instance != null)
                    ClickSettlementUpgradeButton.Instance.GetLeanHover().ManualPointerExit();

                if (BuildingUIUpdater.Instance.selectedBuilding != null)
                    BuildingUIUpdater.Instance.selectedBuilding.ResetHighlightedTile();

                foreach (var button in ObjectHolder.Instance.endTurnButtons) button.DisableButton();

                CellGrid.EndTurn();
            }

            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown("=") || Input.GetKeyDown(KeyCode.KeypadPlus))
                if (SpeedButtons.Instance != null)
                    SpeedButtons.Instance.FastSpeed();


            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown("-") || Input.GetKeyDown(KeyCode.KeypadMinus))
                if (SpeedButtons.Instance != null)
                    SpeedButtons.Instance.NormalSpeed();
        }
    }
}