using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;

public class SpriteButton : MonoBehaviour, IPointerClickHandler,
                            IPointerDownHandler, IPointerEnterHandler,
                            IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private Color _normalColor, _hoverColor, _clickColor, _disabledColor;

    [BoxGroup("Spriterenderer Resource")]
    [SerializeField] private SpriteRenderer _spriteRendererResource;

    public enum ResourceType
    {
        Wood,
        Grain,
        Sheep,
        Stone,
        Horse
    }

    public ResourceType resourceType = ResourceType.Grain;

    [BoxGroup("Resource Prefab")]
    [SerializeField] private GameObject _resourcePrefab;

    private SpriteRenderer _spriteRenderer;
    private HexagonTile    _hexagonTile;
    private Cost           _cost;

    private int costGold, costWood;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _hexagonTile    = GetComponentInParent<HexagonTile>();
        _cost           = Cost.Instance;

        addEventSystem();
    }

    public void OnPointerClick(PointerEventData eventData) { }

    public void OnPointerDown(PointerEventData eventData)
    {
        CalculateCosts();

        if (costGold <= ResourcesController.Instance.gold && costWood <= ResourcesController.Instance.wood)
        {
            _spriteRenderer.color = _clickColor;

            _hexagonTile.HideResourceButtons();

            _hexagonTile.resourceGameObject = Instantiate(_resourcePrefab, _hexagonTile.transform.position,
                                                          Quaternion.Euler(0f, 0f, -90f));

            Building building = _hexagonTile.resourceGameObject.GetComponent<Building>();

            _hexagonTile.settlement.gold  += building.gold;
            _hexagonTile.settlement.wood  += building.wood;
            _hexagonTile.settlement.grain += building.grain;
            _hexagonTile.settlement.sheep += building.sheep;
            _hexagonTile.settlement.stone += building.stone;
            _hexagonTile.settlement.horse += building.horse;

            building.gold  = 0;
            building.wood  = 0;
            building.grain = 0;
            building.sheep = 0;
            building.stone = 0;
            building.horse = 0;

            ResourcesController.Instance.gold -= costGold;
            ResourcesController.Instance.wood -= costWood;

            BuildingUIUpdater.Instance.UpdateResources();
            TextAssigner.Instance.workshopCanvasGroup.DOFade(0f, .2f).SetEase(Ease.InOutQuad);

            if (resourceType == ResourceType.Horse) ResourcesController.Instance.UpdateHorseResource();

            ResourcesController.Instance.UpdateOnlyResourceUIText();

            _spriteRenderer.color = _normalColor;

            TextAssigner.Instance.workshopCanvasGroup.DOFade(0f, .2f).SetEase(Ease.InOutQuad);

            if (_hexagonTile.settlement != null) _hexagonTile.HideResourceButtons();

            ObjectHolder.Instance.UpdateAllButtonStates();
            
            AudioController.Instance.SFXBuildWorkshop();
        }
    }

    public void CalculateCosts()
    {
        switch (resourceType)
        {
            case ResourceType.Grain:
                UpdateCostsVariables(_cost.goldFarm, _cost.woodFarm);
                break;
            case ResourceType.Wood:
                UpdateCostsVariables(_cost.goldLumberCamp, _cost.woodLumberCamp);
                break;
            case ResourceType.Sheep:
                UpdateCostsVariables(_cost.goldSheepBarn, _cost.woodSheepBarn);
                break;
            case ResourceType.Stone:
                UpdateCostsVariables(_cost.goldQuarry, _cost.woodQuarry);
                break;
            case ResourceType.Horse:
                UpdateCostsVariables(_cost.goldStables, _cost.woodStables);
                break;
        }

        if (costGold > ResourcesController.Instance.gold || costWood > ResourcesController.Instance.wood)
        {
            _spriteRenderer.color         = _disabledColor;
            _spriteRendererResource.color = _disabledColor;
        }
        else if (costGold <= ResourcesController.Instance.gold && costWood <= ResourcesController.Instance.wood)
        {
            _spriteRenderer.color         = _normalColor;
            _spriteRendererResource.color = _normalColor;
        }
    }

    private void UpdateCostsVariables(int gold, int wood)
    {
        costGold = gold;
        costWood = wood;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        CalculateCosts();

        if (costGold <= ResourcesController.Instance.gold && costWood <= ResourcesController.Instance.wood)
            _spriteRenderer.color = _hoverColor;

        TextAssigner.Instance.UpdateWorkshopText(resourceType);

        TextAssigner.Instance.workshopCanvasGroup.DOFade(1f, .2f).SetEase(Ease.InOutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CalculateCosts();

        if (costGold <= ResourcesController.Instance.gold && costWood <= ResourcesController.Instance.wood)
            _spriteRenderer.color = _normalColor;

        TextAssigner.Instance.workshopCanvasGroup.DOFade(0f, .2f).SetEase(Ease.InOutQuad);
    }

    //Add Event System to the Camera
    private void addEventSystem()
    {
        GameObject eventSystem = null;
        GameObject tempObj     = GameObject.Find("EventSystem");
        if (tempObj == null)
        {
            eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
        else
        {
            if ((tempObj.GetComponent<EventSystem>()) == null) { tempObj.AddComponent<EventSystem>(); }

            if ((tempObj.GetComponent<StandaloneInputModule>()) == null)
            {
                tempObj.AddComponent<StandaloneInputModule>();
            }
        }
    }

    private void OnEnable()
    {
        ObjectHolder.Instance.spriteButtons.Add(this);
        CalculateCosts();
    }

    private void OnDisable()
    {
        ObjectHolder.Instance.spriteButtons.Remove(this);
        _spriteRenderer.color         = _normalColor;
        _spriteRendererResource.color = _normalColor;
        
        TextAssigner.Instance.workshopCanvasGroup.DOFade(0f, .2f).SetEase(Ease.InOutQuad);
    }
}