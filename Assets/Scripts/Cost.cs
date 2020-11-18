using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Cost : MonoBehaviour
{
    private static Cost _instance;
    public static  Cost Instance => _instance;

    [BoxGroup("Settlement Required Resources - Gold")]
    public int goldHamlet, goldVillage, goldTown, goldCity;

    [BoxGroup("Settlement Required Resources - Wood")]
    public int woodHamlet, woodVillage, woodTown, woodCity;

    [BoxGroup("Settlement Required Resources - Grain")]
    public int grainHamlet, grainVillage, grainTown, grainCity;

    [BoxGroup("Settlement Required Resources - Sheep")]
    public int sheepHamlet, sheepVillage, sheepTown, sheepCity;

    [BoxGroup("Settlement Required Resources - Stone")]
    public int stoneHamlet, stoneVillage, stoneTown, stoneCity;

    [BoxGroup("Stronghold Required Resources - Gold")]
    public int goldOutpost, goldCastle;

    [BoxGroup("Stronghold Required Resources - Wood")]
    public int woodOutpost, woodCastle;
    
    [BoxGroup("Stronghold Required Resources - Grain")]
    public int grainOutpost, grainCastle;

    [BoxGroup("Stronghold Required Resources - Sheep")]
    public int sheepOutpost, sheepCastle;

    [BoxGroup("Stronghold Required Resources - Stone")]
    public int stoneOutpost, stoneCastle;

    [BoxGroup("Stronghold Gold Upkeep")]
    public int outpostGoldUpkeep, castleGoldUpkeep;

    [BoxGroup("Divider")] [HorizontalLine(color: EColor.Green)]
    [SerializeField] private string divide = "Divider";

    [BoxGroup("Settler")]
    public int goldSettler, goldSettlerMultiplier, woodSettler, grainSettler, sheepSettler, stoneSettler, horseSettler;

    [BoxGroup("Villager")]
    public int goldVillager, woodVillager, grainVillager, sheepVillager, stoneVillager, horseVillager;

    [BoxGroup("Pikeman")]
    public int goldPikeman, woodPikeman, grainPikeman, sheepPikeman, stonePikeman, horsePikeman;

    [BoxGroup("Archer")]
    public int goldArcher, woodArcher, grainArcher, sheepArcher, stoneArcher, horseArcher;

    [BoxGroup("Footman")]
    public int goldFootman, woodFootman, grainFootman, sheepFootman, stoneFootman, horseFootman;

    [BoxGroup("Hussar")]
    public int goldHussar, woodHussar, grainHussar, sheepHussar, stoneHussar, horseHussar;

    [BoxGroup("Knight")]
    public int goldKnight, woodKnight, grainKnight, sheepKnight, stoneKnight, horseKnight;

    [BoxGroup("Armoured Archer")]
    public int goldArmouredArcher,
               woodArmouredArcher,
               grainArmouredArcher,
               sheepArmouredArcher,
               stoneArmouredArcher,
               horseArmouredArcher;

    [BoxGroup("Mounted Knight")]
    public int goldMountedKnight,
               woodMountedKnight,
               grainMountedKnight,
               sheepMountedKnight,
               stoneMountedKnight,
               horseMountedKnight;

    [BoxGroup("Paladin")]
    public int goldPaladin, woodPaladin, grainPaladin, sheepPaladin, stonePaladin, horsePaladin;

    [BoxGroup("Mounted Paladin")]
    public int goldMountedPaladin,
               woodMountedPaladin,
               grainMountedPaladin,
               sheepMountedPaladin,
               stoneMountedPaladin,
               horseMountedPaladin;

    [BoxGroup("King")]
    public int goldKing, woodKing, grainKing, sheepKing, stoneKing, horseKing;

    [BoxGroup("Worker")]
    public int goldWorker, woodWorker, grainWorker, sheepWorker, stoneWorker, horseWorker;

    [BoxGroup("Divider Upgrades")] [HorizontalLine(color: EColor.Green)]
    [SerializeField] private string divideUpgrade = "Divide Upgrades";

    [BoxGroup("Upgrade Pikeman")]
    public int goldUpgradePikeman, woodUpgradePikeman, grainUpgradePikeman, sheepUpgradePikeman;

    [BoxGroup("Upgrade Archer")]
    public int goldUpgradeArcher, woodUpgradeArcher, grainUpgradeArcher, sheepUpgradeArcher;

    [BoxGroup("Upgrade Footman")]
    public int goldUpgradeFootman, woodUpgradeFootman, grainUpgradeFootman, sheepUpgradeFootman;

    [BoxGroup("Upgrade Hussar")]
    public int goldUpgradeHussar, woodUpgradeHussar, grainUpgradeHussar, sheepUpgradeHussar;

    [BoxGroup("Upgrade Knight")]
    public int goldUpgradeKnight, woodUpgradeKnight, grainUpgradeKnight, sheepUpgradeKnight;

    [BoxGroup("Upgrade Armoured Archer")]
    public int goldUpgradeArmouredArcher,
               woodUpgradeArmouredArcher,
               grainUpgradeArmouredArcher,
               sheepUpgradeArmouredArcher;

    [BoxGroup("Upgrade Mounted Knight")]
    public int goldUpgradeMountedKnight,
               woodUpgradeMountedKnight,
               grainUpgradeMountedKnight,
               sheepUpgradeMountedKnight;

    [BoxGroup("Divider Workshop Costs")] [HorizontalLine(color: EColor.Red)]
    [SerializeField] private string divideWorkshop = "Divide Workshops";

    [BoxGroup("Farm")]
    public int goldFarm, woodFarm;

    [BoxGroup("Wood Camp")]
    public int goldLumberCamp, woodLumberCamp;

    [BoxGroup("Sheep Barn")]
    public int goldSheepBarn, woodSheepBarn;

    [BoxGroup("Quarry")]
    public int goldQuarry, woodQuarry;

    [BoxGroup("Stables")]
    public int goldStables, woodStables;
    
    [BoxGroup("Divider Market")] [HorizontalLine(color: EColor.Yellow)]
    [SerializeField] private string divideMarket = "Divide Market";

    [BoxGroup("Market Buy")]
    public int buyWood, buyGrain, buySheep, buyStone;
    
    [BoxGroup("Market Sell")]
    public int sellWood, sellGrain, sellSheep, sellStone;
    
    [BoxGroup("Market Multiplier")]
    public int marketPricesMultiplier = 1;
    
    [BoxGroup("Market Fee Reduce Factor")]
    public int marketFeeReduceFactor = 0;

    //[HideInInspector]
    public int unitToSpawnGold, unitToSpawnWood, unitToSpawnGrain, unitToSpawnSheep, unitToSpawnStone, unitToSpawnHorse;

    private void Awake()
    {
        if (_instance == null) _instance = this;
    }

    public void UpdateMarketPriceFee()
    {
        marketFeeReduceFactor = 0;
        
        foreach (Dockyard dockyard in ObjectHolder.Instance.dockyards) { marketFeeReduceFactor++; }
    }
}