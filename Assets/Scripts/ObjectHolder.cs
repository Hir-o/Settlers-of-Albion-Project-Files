using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TbsFramework.Grid;
using TbsFramework.Gui;
using UnityEngine;

public class ObjectHolder : MonoBehaviour
{
    private static ObjectHolder _instance;
    public static  ObjectHolder Instance => _instance;

    [HideInInspector]
    public CellGrid cellGrid;

    [BoxGroup("Unit Health Text")]
    public UnitHealthChangeText unitHealthText;

    [BoxGroup("Building")]
    public GameObject settlement, outpost;

    [BoxGroup("Buildings")]
    public List<Building> buildings = new List<Building>();

    [HideInInspector]
    public Camera mainCamera;

    [HideInInspector]
    public UnitController purchasedUnit;

    [HideInInspector]
    public GameObject purchasedUnitGhost;

    [HideInInspector]
    public ClickFoundCityButton foundCityButton;

    [HideInInspector]
    public ClickFoundOutpost foundOutpostButton;

    [HideInInspector]
    public ClickHealButton healButton;

    [HideInInspector]
    public ClickMarketButton marketButton;

    [HideInInspector]
    public List<ClickDisbandButton> disbandButtons = new List<ClickDisbandButton>();

    [HideInInspector]
    public List<ClickUpgradeUnitButton> upgradeUnitButtons = new List<ClickUpgradeUnitButton>();

    [HideInInspector]
    public List<ClickUnitButton> unitButtons = new List<ClickUnitButton>();

    [HideInInspector]
    public List<SpriteButton> spriteButtons = new List<SpriteButton>();

    [HideInInspector]
    public List<ClickMarketBuyResourceButton> marketBuyButtons = new List<ClickMarketBuyResourceButton>();

    [HideInInspector]
    public List<ClickMarketSellResourceButton> marketSellButtons = new List<ClickMarketSellResourceButton>();

    [HideInInspector]
    public List<ClickEndTurnButton> endTurnButtons = new List<ClickEndTurnButton>();

    [HideInInspector]
    public List<EnemySpawner> enemySpawners = new List<EnemySpawner>();

    [HideInInspector]
    public List<Dockyard> dockyards = new List<Dockyard>();

    [HideInInspector]
    public List<BridgeSpawner> bridgeSpawners = new List<BridgeSpawner>();

    [HideInInspector]
    public List<SpawnSpecialUnit> specialUnitSpawners = new List<SpawnSpecialUnit>();

    [HideInInspector]
    public List<GhostUnitUpdater> unitGhosts = new List<GhostUnitUpdater>();

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        mainCamera = Camera.main;
    }

    public void UpdateAllButtonStates()
    {
        foreach (ClickUnitButton btnUnit in Instance.unitButtons)
        {
            btnUnit.CalculateCosts();
            btnUnit.UpdateButtonState();
        }

        foreach (ClickUpgradeUnitButton btnUnitUpgrade in Instance.upgradeUnitButtons)
        {
            btnUnitUpgrade.CalculateCosts();
            btnUnitUpgrade.UpdateButtonState();
        }

        foreach (SpriteButton button in Instance.spriteButtons.Where(button => button != null)) button.CalculateCosts();

        foreach (var button in Instance.marketBuyButtons) button.UpdateButtonState();

        foreach (var button in Instance.marketSellButtons) button.UpdateButtonState();

        if (ClickSettlementUpgradeButton.Instance != null)
        {
            ClickSettlementUpgradeButton.Instance.CalculateCosts();
            ClickSettlementUpgradeButton.Instance.UpdateButtonState();
        }
    }
}