using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TbsFramework.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitUIUpdater : MonoBehaviour
{
    private static UnitUIUpdater _instance;
    public static  UnitUIUpdater Instance => _instance;

    [BoxGroup("Panels")]
    public GameObject panelUnit;

    [BoxGroup("Unit Description")]
    [SerializeField] private TextMeshProUGUI _tmpName, _tmpDescription;

    [BoxGroup("Unit Image")]
    [SerializeField] private TextMeshProUGUI _tmpLevel;

    [BoxGroup("Unit Image")]
    [SerializeField] private GameObject _unitCard, _settlersCard, _workersCard;

    [BoxGroup("Unit Image")]
    [SerializeField] private List<Image> _images = new List<Image>();

    [BoxGroup("Unit Stats")]
    [SerializeField] private TextMeshProUGUI _tmpHealth, _tmpAttack, _tmpDefense, _tmpActionPoints;

    [BoxGroup("Unit Upkeep")]
    [SerializeField] private TextMeshProUGUI _tmpGold, _tmpGrain, _tmpSheep, _tmpHorse;

    [BoxGroup("Unit Upkeep")]
    [SerializeField] private GameObject _goldGameObject, _grainGameObject, _sheepGameObject, _horseGameObject;

    [BoxGroup("Skills")]
    [SerializeField] private GameObject _unitSkillsTab, _settlersSkillTab, _workerSkillTab;

    [BoxGroup("Resources")]
    [SerializeField] private TextMeshProUGUI _tmpResourceGold,
                                             _tmpResourceWood,
                                             _tmpResourceGrain,
                                             _tmpResourceSheep,
                                             _tmpResourceStone,
                                             _tmpResourceHorse;

    [HideInInspector]
    public UnitController selectedUnit;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        panelUnit.SetActive(false);

        if (_settlersSkillTab.activeSelf) _settlersSkillTab.SetActive(false);

        ResourcesController.Instance.tmpGold  = _tmpResourceGold;
        ResourcesController.Instance.tmpWood  = _tmpResourceWood;
        ResourcesController.Instance.tmpGrain = _tmpResourceGrain;
        ResourcesController.Instance.tmpSheep = _tmpResourceSheep;
        ResourcesController.Instance.tmpStone = _tmpResourceStone;
        ResourcesController.Instance.tmpHorse = _tmpResourceHorse;
    }

    private void Start()
    {
        ResourcesController.Instance.UpdateOnlyResourceUIText();
    }

    public void UpdateUnitPanel(UnitController unitController,
                                Unit.UnitType unitType,
                                string name,
                                string description,
                                UnitController.Level level,
                                SpriteRenderer[] spriteRenderers)
    {
        if (unitType != Unit.UnitType.Building && unitController.PlayerNumber == 0)
        {
            selectedUnit = unitController;

            if (BuildingUIUpdater.Instance.panelBuilding.activeSelf)
                BuildingUIUpdater.Instance.panelBuilding.SetActive(false);

            panelUnit.SetActive(true);

            UpdateDescription(name, description);
            UpdateStats(unitController);
            UpdateUpkeep(unitController);

            if (unitType != Unit.UnitType.Settler && unitType != Unit.UnitType.Worker)
                UpdateCard(unitType, level, spriteRenderers);
            else if (unitType == Unit.UnitType.Settler)
                UpdateCardSettler();
            else if (unitType == Unit.UnitType.Worker) UpdateCardWorker();
        }
    }

    public void UpdateDescription(string name, string description)
    {
        _tmpName.text        = name;
        _tmpDescription.text = "\"" + description + "\"";
    }

    public void UpdateCard(Unit.UnitType unitType, UnitController.Level level, SpriteRenderer[] spriteRenderers)
    {
        if (_unitCard.activeSelf == false)
        {
            _unitCard.SetActive(true);
            _unitSkillsTab.SetActive(true);
        }

        if (_settlersCard.activeSelf)
        {
            _settlersCard.SetActive(false);
            _settlersSkillTab.SetActive(false);
        }

        if (_workersCard.activeSelf)
        {
            _workersCard.SetActive(false);
            _workerSkillTab.SetActive(false);
        }

        int levelIndex = (int) level;

        _tmpLevel.text = (levelIndex + 1).ToString();

        foreach (Image image in _images) { image.gameObject.SetActive(false); }

        for (int i = 0; i <= levelIndex; i++)
        {
            if (_images[i].gameObject.activeSelf == false) _images[i].gameObject.SetActive(true);

            _images[i].sprite = spriteRenderers[i].sprite;
        }
    }

    public void UpdateCardSettler()
    {
        if (_unitCard.activeSelf)
        {
            _unitCard.SetActive(false);
            _unitSkillsTab.SetActive(false);
        }

        if (_settlersCard.activeSelf == false)
        {
            _settlersCard.SetActive(true);
            _settlersSkillTab.SetActive(true);
        }
        
        if (_workersCard.activeSelf)
        {
            _workersCard.SetActive(false);
            _workerSkillTab.SetActive(false);
        }
    }

    public void UpdateCardWorker()
    {
        if (_unitCard.activeSelf)
        {
            _unitCard.SetActive(false);
            _unitSkillsTab.SetActive(false);
        }

        if (_settlersCard.activeSelf)
        {
            _settlersCard.SetActive(false);
            _settlersSkillTab.SetActive(false);
        }
        
        if (_workersCard.activeSelf == false)
        {
            _workersCard.SetActive(true);
            _workerSkillTab.SetActive(true);
        }
    }

    public void UpdateStats(UnitController unitController)
    {
        _tmpHealth.text       = unitController.HitPoints.ToString();
        _tmpAttack.text       = unitController.AttackFactor.ToString();
        _tmpDefense.text      = unitController.DefenceFactor.ToString();
        _tmpActionPoints.text = unitController.ActionPoints.ToString();
    }

    public void UpdateUpkeep(UnitController unitController)
    {
        _tmpGold.text  = unitController.upkeepGold.ToString();
        _tmpGrain.text = unitController.upkeepGrain.ToString();
        _tmpSheep.text = unitController.upkeepSheep.ToString();
        _tmpHorse.text = unitController.upkeepHorse.ToString();

        if (unitController.upkeepGold > 0)
            _goldGameObject.SetActive(true);
        else
            _goldGameObject.SetActive(false);

        if (unitController.upkeepGrain > 0)
            _grainGameObject.SetActive(true);
        else
            _grainGameObject.SetActive(false);

        if (unitController.upkeepSheep > 0)
            _sheepGameObject.SetActive(true);
        else
            _sheepGameObject.SetActive(false);

        if (unitController.upkeepHorse > 0)
            _horseGameObject.SetActive(true);
        else
            _horseGameObject.SetActive(false);
    }
}