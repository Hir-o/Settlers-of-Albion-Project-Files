using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ResourcesController : MonoBehaviour
{
    private static ResourcesController _instance;
    public static  ResourcesController Instance => _instance;

    public  int gold,    wood,    grain,    sheep,    stone, horse;
    private int _goldPT, _woodPT, _grainPT, _sheepPT, _stonePT;

    public TextMeshProUGUI tmpGold, tmpWood, tmpGrain, tmpSheep, tmpStone, tmpHorse;

    public bool isMakingGold, isMakingWood, isMakingGrain, isMakingSheep, isMakingStone, isMakingHorse;

    [BoxGroup("Text Colors")]
    [SerializeField] private Color _colorResourceGain, _colorResourceLoose;

    private string _resourceAmountAsText;

    public void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateHorseResource() // called when a stable is built
    {
        int allHorses = 0;

        foreach (Building building in ObjectHolder.Instance.buildings)
            if (building.isSettlement)
                allHorses += building.horse * (int) building.upgradeLevel;

        foreach (UnitController unit in ObjectHolder.Instance.cellGrid.Units) allHorses -= unit.upkeepHorse;

        horse = allHorses;

        if (horse < 0) horse = 0;

        UpdateResourceUI(horse);
    }

    public void UpdateResources()
    {
        if (Quests.Instance.mainQuestType == Quests.MainQuestType.Battle) { return; }

        int goldBonus = 0, allHorses = 0;

        _goldPT = _woodPT = _grainPT = _sheepPT = _stonePT = 0;

        foreach (Building building in ObjectHolder.Instance.buildings)
        {
            goldBonus = 0;
            
            if (building.isEnemyControlled) continue;

            if (building.isSettlement)
            {
                for (int i = (int) building.upgradeLevel - 1; i > 0; i--) goldBonus += i;

                gold      += building.gold * (int) building.upgradeLevel + goldBonus;
                wood      += building.wood  * (int) building.upgradeLevel;
                grain     += building.grain * (int) building.upgradeLevel;
                sheep     += building.sheep * (int) building.upgradeLevel;
                stone     += building.stone * (int) building.upgradeLevel;
                allHorses += building.horse * (int) building.upgradeLevel;

                _goldPT  += building.gold * (int) building.upgradeLevel + goldBonus;
                _woodPT  += building.wood  * (int) building.upgradeLevel;
                _grainPT += building.grain * (int) building.upgradeLevel;
                _sheepPT += building.sheep * (int) building.upgradeLevel;
                _stonePT += building.stone * (int) building.upgradeLevel;
            }
            else if (building.isStronghold)
            {
                gold    += building.gold;
                _goldPT += building.gold;
            }
        }

        foreach (UnitController unit in ObjectHolder.Instance.cellGrid.Units)
        {
            gold      -= unit.upkeepGold;
            grain     -= unit.upkeepGrain;
            sheep     -= unit.upkeepSheep;
            allHorses -= unit.upkeepHorse;

            _goldPT  -= unit.upkeepGold;
            _grainPT -= unit.upkeepGrain;
            _sheepPT -= unit.upkeepSheep;
        }

        horse = allHorses;

        if (gold  < 0) gold  = 0;
        if (wood  < 0) wood  = 0;
        if (grain < 0) grain = 0;
        if (sheep < 0) sheep = 0;
        if (stone < 0) stone = 0;
        if (horse < 0) horse = 0;

        UpdateResourceUI(horse);
    }

    public void UpdateResourceUI(int horses)
    {
        tmpGold.text  = SetTextContent(gold,  _goldPT);
        tmpWood.text  = SetTextContent(wood,  _woodPT);
        tmpGrain.text = SetTextContent(grain, _grainPT);
        tmpSheep.text = SetTextContent(sheep, _sheepPT);
        tmpStone.text = SetTextContent(stone, _stonePT);
        tmpHorse.text = horses.ToString();
    }

    public void UpdateOnlyResourceUIText()
    {
        if (Quests.Instance.mainQuestType == Quests.MainQuestType.Battle) { return; }

        int goldBonus = 0, allHorses = 0;

        _goldPT = _woodPT = _grainPT = _sheepPT = _stonePT = 0;

        foreach (Building building in ObjectHolder.Instance.buildings)
        {
            goldBonus = 0;
            
            if (building.isEnemyControlled) continue;

            if (building.isSettlement)
            {
                for (int i = (int) building.upgradeLevel - 1; i > 0; i--) goldBonus += i;

                _goldPT   += building.gold * (int) building.upgradeLevel + goldBonus;
                _woodPT   += building.wood  * (int) building.upgradeLevel;
                _grainPT  += building.grain * (int) building.upgradeLevel;
                _sheepPT  += building.sheep * (int) building.upgradeLevel;
                _stonePT  += building.stone * (int) building.upgradeLevel;
                allHorses += building.horse * (int) building.upgradeLevel;
            }
            else if (building.isStronghold) { _goldPT += building.gold; }
        }

        foreach (UnitController unit in ObjectHolder.Instance.cellGrid.Units)
        {
            _goldPT   -= unit.upkeepGold;
            _grainPT  -= unit.upkeepGrain;
            _sheepPT  -= unit.upkeepSheep;
            allHorses -= unit.upkeepHorse;
        }

        isMakingGold  = _goldPT   >= 0;
        isMakingWood  = _woodPT   >= 0;
        isMakingGrain = _grainPT  >= 0;
        isMakingSheep = _sheepPT  >= 0;
        isMakingStone = _stonePT  >= 0;
        isMakingHorse = allHorses >= 0;

        if (allHorses < 0) allHorses = 0;

        UpdateResourceUI(allHorses);
    }

    private string SetTextContent(int resourceAmount, int resourcePT)
    {
        Color  tmpColor;
        string resourceGain;

        if (resourcePT >= 0)
        {
            tmpColor     = _colorResourceGain;
            resourceGain = "+" + resourcePT;
        }
        else
        {
            tmpColor     = _colorResourceLoose;
            resourceGain = resourcePT.ToString();
        }

        _resourceAmountAsText = resourceAmount.ToString();

        if (resourceAmount > 100000)
            _resourceAmountAsText = Math.Round(resourceAmount / 100000f, 1) + "B";
        else if (resourceAmount > 10000)
            _resourceAmountAsText = Math.Round(resourceAmount / 10000f, 1) + "M";
        else if (resourceAmount > 1000) _resourceAmountAsText = Math.Round(resourceAmount / 1000f, 1)  + "K";

        return
            $"{_resourceAmountAsText} <font=\"PatrickHand-Regular SDF UI Outline\"><color=#{ColorUtility.ToHtmlStringRGBA(tmpColor)}><cspace=.1em><voffset=.2em><size=28>{resourceGain}</size></cspace></color></font>";
    }
}