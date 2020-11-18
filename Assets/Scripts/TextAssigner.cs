using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TbsFramework.Units;
using TMPro;
using UnityEngine;

public class TextAssigner : MonoBehaviour
{
    private static TextAssigner _instance;
    public static  TextAssigner Instance => _instance;

    [BoxGroup("Hover Info Rect Transform")]
    [SerializeField] private RectTransform _rectHoverInfoBuilding, _rectHoverInfoUnit;

    [BoxGroup("Colors")]
    [SerializeField] private Color _greenColor, _redColor;

    [BoxGroup("Descriptions Settler/Worker TMP")]
    [SerializeField] private TextMeshProUGUI _tmpDescriptionFoundSettlement,
                                             _tmpDescriptionFoundOutpost,
                                             _tmpDescriptionUpgradeBuilding,
                                             _tmpDescriptionUnit,
                                             _tmpDescriptionUnitUpgrade,
                                             _tmpCustomDescriptionWorkshop;

    [BoxGroup("Workshop Canvas Group")]
    public CanvasGroup workshopCanvasGroup;

    private Cost _costs;

    private int _settlersAndSettlements;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        _costs = Cost.Instance;
    }

    public void UpdateFoundSettlementText()
    {
        _tmpDescriptionFoundSettlement.text = $"<size=110%>Found Hamlet " +
                                              $"\n<size=100%>The hamlet is the smallest settlement type";
    }

    public void UpdateFoundOutpostText()
    {
        _tmpDescriptionFoundOutpost.text =
            $"<size=110%>Found Outpost \n"                                                                +
            $"<size=100%>The outpost provides protection, and enables the recruitment of veteran units\n" +
            $"<align=right>(Upkeep: <sprite=0 index=4><color=#{ColorUtility.ToHtmlStringRGBA(_redColor)}>{_costs.outpostGoldUpkeep}</color>)";
    }

    public void UpdateBuildingText()
    {
        string name = "", goldCost = "", woodCost = "", grainCost = "", sheepCost = "", stoneCost = "";

        if (BuildingUIUpdater.Instance.selectedBuilding.isStronghold)
        {
            goldCost  = UpdateResourceColor(_costs.goldCastle,  ResourcesController.Instance.gold,  4, goldCost);
            woodCost  = UpdateResourceColor(_costs.woodCastle,  ResourcesController.Instance.wood,  5, woodCost);
            sheepCost = UpdateResourceColor(_costs.sheepCastle, ResourcesController.Instance.sheep, 7, sheepCost);
            stoneCost = UpdateResourceColor(_costs.stoneCastle, ResourcesController.Instance.stone, 8, stoneCost);

            _rectHoverInfoBuilding.sizeDelta = new Vector2(20.1f, 5.7f);

            _tmpDescriptionUpgradeBuilding.text =
                $"<size=110%>Upgrade to Castle<size=100%> (Cost: {goldCost}{woodCost}{sheepCost}{stoneCost})\n"                           +
                $"The castle provides protection from the most dangerous enemies, and enables the recruitment of the most elite forces\n" +
                $"<align=right>(Upkeep: <color=#{ColorUtility.ToHtmlStringRGBA(_redColor)}>{_costs.castleGoldUpkeep}</color><sprite=0 index=4>)";
        }
        else if (BuildingUIUpdater.Instance.selectedBuilding.isSettlement)
        {
            _rectHoverInfoBuilding.sizeDelta = new Vector2(18f, 3.3f);

            switch (BuildingUIUpdater.Instance.selectedBuilding.upgradeLevel)
            {
                case Building.UpgradeLevel.Hamlet:
                    name = "Village";

                    goldCost = UpdateResourceColor(_costs.goldVillage, ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodVillage, ResourcesController.Instance.wood, 5, woodCost);
                    grainCost = UpdateResourceColor(_costs.grainVillage, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    break;
                case Building.UpgradeLevel.Village:
                    name = "Town";

                    goldCost  = UpdateResourceColor(_costs.goldTown,  ResourcesController.Instance.gold,  4, goldCost);
                    grainCost = UpdateResourceColor(_costs.grainTown, ResourcesController.Instance.grain, 6, grainCost);
                    stoneCost = UpdateResourceColor(_costs.stoneTown, ResourcesController.Instance.stone, 8, stoneCost);
                    break;
                case Building.UpgradeLevel.Town:
                    name = "City";

                    goldCost  = UpdateResourceColor(_costs.goldCity,  ResourcesController.Instance.gold,  4, goldCost);
                    grainCost = UpdateResourceColor(_costs.grainCity, ResourcesController.Instance.grain, 6, grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepCity, ResourcesController.Instance.sheep, 7, sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stoneCity, ResourcesController.Instance.stone, 8, stoneCost);

                    _rectHoverInfoBuilding.sizeDelta = new Vector2(20.1f, 3.3f);
                    break;
            }

            _tmpDescriptionUpgradeBuilding.text =
                $"<size=110%>Upgrade to {name}<size=100%> (Cost {goldCost}{woodCost}{grainCost}{sheepCost}{stoneCost})\n" +
                $"Increase taxes and resource productions\n";
            ;
        }
    }

    public void UpdateUnitText(UnitController unit)
    {
        string name        = "",
               description = "",
               goldCost    = "",
               woodCost    = "",
               grainCost   = "",
               horseCost   = "",
               sheepCost   = "",
               stoneCost   = "";

        int hitpoints = unit.MaxHitPoints;

        if (unit.unitType != Unit.UnitType.Building)
        {
            name        = unit.GetName();
            description = unit.GetDescription();

            switch (unit.unitType)
            {
                case Unit.UnitType.Settler:
                    _settlersAndSettlements = 0;

                    foreach (Building building in ObjectHolder.Instance.buildings)
                        if (building.isSettlement && building.GetUnitController().PlayerNumber == 0)
                            _settlersAndSettlements++;

                    foreach (UnitController tempUnit in ObjectHolder.Instance.cellGrid.Units)
                        if (tempUnit.PlayerNumber == 0 && tempUnit.unitType == Unit.UnitType.Settler)
                            _settlersAndSettlements++;

                    goldCost = UpdateResourceColor(_costs.goldSettler + (_settlersAndSettlements * _costs.goldSettlerMultiplier), ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodSettler, ResourcesController.Instance.wood, 5, woodCost);
                    grainCost = UpdateResourceColor(_costs.grainSettler, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepSettler, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stoneSettler, ResourcesController.Instance.stone, 8,
                                                    stoneCost);

                    break;
                case Unit.UnitType.Villager:
                    goldCost = UpdateResourceColor(_costs.goldVillager, ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodVillager, ResourcesController.Instance.wood, 5, woodCost);
                    grainCost = UpdateResourceColor(_costs.grainVillager, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepVillager, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stoneVillager, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    break;
                case Unit.UnitType.Pikeman:
                    goldCost = UpdateResourceColor(_costs.goldPikeman, ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodPikeman, ResourcesController.Instance.wood, 5, woodCost);
                    grainCost = UpdateResourceColor(_costs.grainPikeman, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepPikeman, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stonePikeman, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    hitpoints = unit.MaxHitPoints * UnitLevelController.Instance.pikemanLevel;
                    break;
                case Unit.UnitType.Archer:
                    goldCost = UpdateResourceColor(_costs.goldArcher, ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodArcher, ResourcesController.Instance.wood, 5, woodCost);
                    grainCost = UpdateResourceColor(_costs.grainArcher, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepArcher, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stoneArcher, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    hitpoints = unit.MaxHitPoints * UnitLevelController.Instance.archerLevel;
                    break;
                case Unit.UnitType.Footman:
                    goldCost = UpdateResourceColor(_costs.goldFootman, ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodFootman, ResourcesController.Instance.wood, 5, woodCost);
                    grainCost = UpdateResourceColor(_costs.grainFootman, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepFootman, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stoneFootman, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    hitpoints = unit.MaxHitPoints * UnitLevelController.Instance.footmanLevel;
                    break;
                case Unit.UnitType.Hussar:
                    goldCost = UpdateResourceColor(_costs.goldHussar, ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodHussar, ResourcesController.Instance.wood, 5, woodCost);
                    grainCost = UpdateResourceColor(_costs.grainHussar, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepHussar, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    horseCost = UpdateResourceColor(_costs.horseHussar, ResourcesController.Instance.horse, 9,
                                                    horseCost);
                    stoneCost = UpdateResourceColor(_costs.stoneHussar, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    hitpoints = unit.MaxHitPoints * UnitLevelController.Instance.hussarLevel;
                    break;
                case Unit.UnitType.Knight:
                    goldCost = UpdateResourceColor(_costs.goldKnight, ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodKnight, ResourcesController.Instance.wood, 5, woodCost);
                    grainCost = UpdateResourceColor(_costs.grainKnight, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepKnight, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stoneKnight, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    hitpoints = unit.MaxHitPoints * UnitLevelController.Instance.knightLevel;
                    break;
                case Unit.UnitType.ArmouredArcher:
                    goldCost = UpdateResourceColor(_costs.goldArmouredArcher, ResourcesController.Instance.gold, 4,
                                                   goldCost);
                    woodCost = UpdateResourceColor(_costs.woodArmouredArcher, ResourcesController.Instance.wood, 5,
                                                   woodCost);
                    grainCost = UpdateResourceColor(_costs.grainArmouredArcher, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepArmouredArcher, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stoneArmouredArcher, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    hitpoints = unit.MaxHitPoints * UnitLevelController.Instance.armouredArcherLevel;
                    break;
                case Unit.UnitType.MountedKnight:
                    goldCost = UpdateResourceColor(_costs.goldMountedKnight, ResourcesController.Instance.gold, 4,
                                                   goldCost);
                    woodCost = UpdateResourceColor(_costs.woodMountedKnight, ResourcesController.Instance.wood, 5,
                                                   woodCost);
                    grainCost = UpdateResourceColor(_costs.grainMountedKnight, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepMountedKnight, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    horseCost = UpdateResourceColor(_costs.horseMountedKnight, ResourcesController.Instance.horse, 9,
                                                    horseCost);
                    stoneCost = UpdateResourceColor(_costs.stoneMountedKnight, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    hitpoints = unit.MaxHitPoints * UnitLevelController.Instance.mountedKnightLevel;
                    break;
                case Unit.UnitType.Paladin:
                    goldCost = UpdateResourceColor(_costs.goldPaladin, ResourcesController.Instance.gold, 4,
                                                   goldCost);
                    woodCost = UpdateResourceColor(_costs.woodPaladin, ResourcesController.Instance.wood, 5,
                                                   woodCost);
                    grainCost = UpdateResourceColor(_costs.grainPaladin, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepPaladin, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stonePaladin, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    break;
                case Unit.UnitType.MountedPaladin:
                    goldCost = UpdateResourceColor(_costs.goldMountedPaladin, ResourcesController.Instance.gold, 4,
                                                   goldCost);
                    woodCost = UpdateResourceColor(_costs.woodMountedPaladin, ResourcesController.Instance.wood, 5,
                                                   woodCost);
                    grainCost = UpdateResourceColor(_costs.grainMountedPaladin, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepMountedPaladin, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    horseCost = UpdateResourceColor(_costs.horseMountedPaladin, ResourcesController.Instance.horse, 9,
                                                    horseCost);
                    stoneCost = UpdateResourceColor(_costs.stoneHussar, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    break;
                case Unit.UnitType.King:
                    goldCost = UpdateResourceColor(_costs.goldKing, ResourcesController.Instance.gold, 4,
                                                   goldCost);
                    woodCost = UpdateResourceColor(_costs.woodKing, ResourcesController.Instance.wood, 5,
                                                   woodCost);
                    grainCost = UpdateResourceColor(_costs.grainKing, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepKing, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stoneKing, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    break;
                case Unit.UnitType.Worker:
                    goldCost = UpdateResourceColor(_costs.goldWorker, ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodWorker, ResourcesController.Instance.wood, 5, woodCost);
                    grainCost = UpdateResourceColor(_costs.grainWorker, ResourcesController.Instance.grain, 6,
                                                    grainCost);
                    sheepCost = UpdateResourceColor(_costs.sheepWorker, ResourcesController.Instance.sheep, 7,
                                                    sheepCost);
                    stoneCost = UpdateResourceColor(_costs.stoneWorker, ResourcesController.Instance.stone, 8,
                                                    stoneCost);
                    break;
            }

            _tmpDescriptionUnit.text =
                $"<size=110%>Recruit {name}<size=100%> (Cost: {goldCost}{woodCost}{grainCost}{sheepCost}{horseCost}{stoneCost})\n"                                                           +
                $"{description}\n"                                                                                                                                                +
                $"<size=110%>{hitpoints}<sprite=0 index=0> {unit.AttackFactor}<sprite=0 index=1> {unit.DefenceFactor}<sprite=0 index=2> {unit.ActionPoints}<sprite=0 index=3> \n" +
                $"<align=right><size=100%>(Upkeep: {UpdateUpkeepResource(unit.upkeepGold, 4)}{UpdateUpkeepResource(unit.upkeepGrain, 6)}{UpdateUpkeepResource(unit.upkeepSheep, 7)}{UpdateUpkeepResource(unit.upkeepHorse, 9)})";
        }
    }

    public void UpdateUnitUpgradeText(UnitController unit)
    {
        string name = unit.GetName(), goldCost = "", woodCost = "", grainCost = "", sheepCost = "";

        if (unit.unitType != Unit.UnitType.Building)
        {
            switch (unit.unitType)
            {
                case Unit.UnitType.Pikeman:
                    goldCost =
                        UpdateResourceColor(_costs.goldUpgradePikeman * UnitLevelController.Instance.pikemanLevel,
                                            ResourcesController.Instance.gold, 4, goldCost);
                    woodCost =
                        UpdateResourceColor(_costs.woodUpgradePikeman * UnitLevelController.Instance.pikemanLevel,
                                            ResourcesController.Instance.wood, 5, woodCost);
                    grainCost =
                        UpdateResourceColor(_costs.grainUpgradePikeman * UnitLevelController.Instance.pikemanLevel,
                                            ResourcesController.Instance.grain, 6, grainCost);
                    sheepCost =
                        UpdateResourceColor(_costs.sheepUpgradePikeman * UnitLevelController.Instance.pikemanLevel,
                                            ResourcesController.Instance.sheep, 7, sheepCost);
                    break;
                case Unit.UnitType.Archer:
                    goldCost = UpdateResourceColor(_costs.goldUpgradeArcher * UnitLevelController.Instance.archerLevel,
                                                   ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodUpgradeArcher * UnitLevelController.Instance.archerLevel,
                                                   ResourcesController.Instance.wood, 5, woodCost);
                    grainCost =
                        UpdateResourceColor(_costs.grainUpgradeArcher * UnitLevelController.Instance.archerLevel,
                                            ResourcesController.Instance.grain, 6, grainCost);
                    sheepCost =
                        UpdateResourceColor(_costs.sheepUpgradeArcher * UnitLevelController.Instance.archerLevel,
                                            ResourcesController.Instance.sheep, 7, sheepCost);
                    break;
                case Unit.UnitType.Footman:
                    goldCost =
                        UpdateResourceColor(_costs.goldUpgradeFootman * UnitLevelController.Instance.footmanLevel,
                                            ResourcesController.Instance.gold, 4, goldCost);
                    woodCost =
                        UpdateResourceColor(_costs.woodUpgradeFootman * UnitLevelController.Instance.footmanLevel,
                                            ResourcesController.Instance.wood, 5, woodCost);
                    grainCost =
                        UpdateResourceColor(_costs.grainUpgradeFootman * UnitLevelController.Instance.footmanLevel,
                                            ResourcesController.Instance.grain, 6, grainCost);
                    sheepCost =
                        UpdateResourceColor(_costs.sheepUpgradeFootman * UnitLevelController.Instance.footmanLevel,
                                            ResourcesController.Instance.sheep, 7, sheepCost);
                    break;
                case Unit.UnitType.Hussar:
                    goldCost = UpdateResourceColor(_costs.goldUpgradeHussar * UnitLevelController.Instance.hussarLevel,
                                                   ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodUpgradeHussar * UnitLevelController.Instance.hussarLevel,
                                                   ResourcesController.Instance.wood, 5, woodCost);
                    grainCost =
                        UpdateResourceColor(_costs.grainUpgradeHussar * UnitLevelController.Instance.hussarLevel,
                                            ResourcesController.Instance.grain, 6, grainCost);
                    sheepCost =
                        UpdateResourceColor(_costs.sheepUpgradeHussar * UnitLevelController.Instance.hussarLevel,
                                            ResourcesController.Instance.sheep, 7, sheepCost);
                    break;
                case Unit.UnitType.Knight:
                    goldCost = UpdateResourceColor(_costs.goldUpgradeKnight * UnitLevelController.Instance.knightLevel,
                                                   ResourcesController.Instance.gold, 4, goldCost);
                    woodCost = UpdateResourceColor(_costs.woodUpgradeKnight * UnitLevelController.Instance.knightLevel,
                                                   ResourcesController.Instance.wood, 5, woodCost);
                    grainCost =
                        UpdateResourceColor(_costs.grainUpgradeKnight * UnitLevelController.Instance.knightLevel,
                                            ResourcesController.Instance.grain, 6, grainCost);
                    sheepCost =
                        UpdateResourceColor(_costs.sheepUpgradeKnight * UnitLevelController.Instance.knightLevel,
                                            ResourcesController.Instance.sheep, 7, sheepCost);
                    break;
                case Unit.UnitType.ArmouredArcher:
                    goldCost =
                        UpdateResourceColor(_costs.goldUpgradeArmouredArcher * UnitLevelController.Instance.armouredArcherLevel,
                                            ResourcesController.Instance.gold, 4, goldCost);
                    woodCost =
                        UpdateResourceColor(_costs.woodUpgradeArmouredArcher * UnitLevelController.Instance.armouredArcherLevel,
                                            ResourcesController.Instance.wood, 5, woodCost);
                    grainCost =
                        UpdateResourceColor(_costs.grainUpgradeArmouredArcher * UnitLevelController.Instance.armouredArcherLevel,
                                            ResourcesController.Instance.grain, 6, grainCost);
                    sheepCost =
                        UpdateResourceColor(_costs.sheepUpgradeArmouredArcher * UnitLevelController.Instance.armouredArcherLevel,
                                            ResourcesController.Instance.sheep, 7, sheepCost);
                    break;
                case Unit.UnitType.MountedKnight:
                    goldCost =
                        UpdateResourceColor(_costs.goldUpgradeMountedKnight * UnitLevelController.Instance.mountedKnightLevel,
                                            ResourcesController.Instance.gold, 4, goldCost);
                    woodCost =
                        UpdateResourceColor(_costs.woodUpgradeMountedKnight * UnitLevelController.Instance.mountedKnightLevel,
                                            ResourcesController.Instance.wood, 5, woodCost);
                    grainCost =
                        UpdateResourceColor(_costs.grainUpgradeMountedKnight * UnitLevelController.Instance.mountedKnightLevel,
                                            ResourcesController.Instance.grain, 6, grainCost);
                    sheepCost =
                        UpdateResourceColor(_costs.sheepUpgradeMountedKnight * UnitLevelController.Instance.mountedKnightLevel,
                                            ResourcesController.Instance.sheep, 7, sheepCost);
                    break;
            }

            _tmpDescriptionUnitUpgrade.text =
                $"<size=110%>Upgrade {name}<size=100%> (Cost: {goldCost}{woodCost}{grainCost}{sheepCost})\n" +
                $"Doubles the health of the {name}\n";
        }
    }

    public void UpdateWorkshopText(SpriteButton.ResourceType resourceType)
    {
        string name        = "",
               description = "",
               goldCost    = "",
               woodCost    = "";

        switch (resourceType)
        {
            case SpriteButton.ResourceType.Wood:
                goldCost =
                    UpdateResourceColor(_costs.goldLumberCamp, ResourcesController.Instance.gold, 4, goldCost);
                woodCost =
                    UpdateResourceColor(_costs.woodLumberCamp, ResourcesController.Instance.wood, 5, woodCost);
                name        = "Lumber Camp";
                description = "Produces Wood <sprite=1 index=5>";
                break;
            case SpriteButton.ResourceType.Grain:
                goldCost    = UpdateResourceColor(_costs.goldFarm, ResourcesController.Instance.gold, 4, goldCost);
                woodCost    = UpdateResourceColor(_costs.woodFarm, ResourcesController.Instance.wood, 5, woodCost);
                name        = "Farm";
                description = "Produces Grain <sprite=1 index=6>";
                break;
            case SpriteButton.ResourceType.Sheep:
                goldCost    = UpdateResourceColor(_costs.goldSheepBarn, ResourcesController.Instance.gold, 4, goldCost);
                woodCost    = UpdateResourceColor(_costs.woodSheepBarn, ResourcesController.Instance.wood, 5, woodCost);
                name        = "Sheep Barn";
                description = "Produces Sheep <sprite=1 index=7>";
                break;
            case SpriteButton.ResourceType.Stone:
                goldCost    = UpdateResourceColor(_costs.goldQuarry, ResourcesController.Instance.gold, 4, goldCost);
                woodCost    = UpdateResourceColor(_costs.woodQuarry, ResourcesController.Instance.wood, 5, woodCost);
                name        = "Stone Quarry";
                description = "Produces Stone <sprite=1 index=8>";
                break;
            case SpriteButton.ResourceType.Horse:
                goldCost    = UpdateResourceColor(_costs.goldStables, ResourcesController.Instance.gold, 4, goldCost);
                woodCost    = UpdateResourceColor(_costs.woodStables, ResourcesController.Instance.wood, 5, woodCost);
                name        = "Stable";
                description = "Produces Horses <sprite=1 index=9>";
                break;
        }

        _tmpCustomDescriptionWorkshop.text = $"<size=110%>Build {name}<size=100%> (Cost: {goldCost}{woodCost})\n" +
                                             $"{description}";
    }

    private string UpdateResourceColor(int value, int resourceStockpiled, int spriteIndex, string resource)
    {
        if (value == 0) return string.Empty;

        if (value > resourceStockpiled)
            resource =
                $"<sprite=0 index={spriteIndex}><color=#{ColorUtility.ToHtmlStringRGBA(_redColor)}>{value}</color>";
        else
            resource =
                $"<sprite=0 index={spriteIndex}><color=#{ColorUtility.ToHtmlStringRGBA(_greenColor)}>{value}</color>";

        return resource;
    }

    private string UpdateUpkeepResource(int value, int spriteIndex)
    {
        if (value == 0) return string.Empty;

        return $"<sprite=0 index={spriteIndex}><color=#{ColorUtility.ToHtmlStringRGBA(_redColor)}>{value}</color>";
    }
}