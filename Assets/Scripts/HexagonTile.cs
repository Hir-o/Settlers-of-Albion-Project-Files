using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TbsFramework.Cells;
using UnityEngine;

public class HexagonTile : Hexagon
{
    public GroundType groundType;
    public bool       isSettlementTile;
    public bool       isSkyTaken; //Indicates if a flying unit is occupying the cell.
    public bool       isMountain;
    public bool       isHazard;
    public bool       isDockTile;
    public bool       hasShip;

    private Vector3 _dimensions = new Vector3(1.72f, 1.48f, 0f);

    [BoxGroup("Highlighter")]
    [SerializeField] private GameObject _highlighter;

    [BoxGroup("Highlighter")]
    [SerializeField] private SpriteRenderer _highlighterSpriteRenderer;

    [BoxGroup("Highlight Sprite Colors")]
    [SerializeField] private Color _markAsReachableColor, _markAsPathColor, _markAsHighlightedColor;

    [BoxGroup("Settlement")]
    public Building settlement;

    [BoxGroup("Dockyard")]
    public Dockyard dockyard;

    [BoxGroup("Resource Buttons")]
    [SerializeField] private GameObject _btnWood, _btnGrain, _btnSheep, _btnStone, _btnHorse;

    [HideInInspector]
    public GameObject resourceGameObject;

    public enum ResourceType
    {
        None,
        Wood,
        Grain,
        Sheep,
        Stone,
        Horse
    }

    public ResourceType resourceType = ResourceType.None;

    public SpriteRenderer spriteRendererResource;

    private Collider2D _collider;

    public void Start()
    {
//        if (groundType == GroundType.Water && hasShip == false)
//        {
//            _collider         = GetComponent<Collider2D>();
//            _collider.enabled = false;
//        }

        SetColor(new Color(1, 1, 1, 0));
    }

    public override void MarkAsReachable()   { SetColor(_markAsReachableColor); }
    public override void MarkAsPath()        { SetColor(_markAsPathColor); }
    public override void MarkAsHighlighted() { SetColor(_markAsHighlightedColor); }

    public override void UnMark()
    {
        if (isUnitSpawnable) return;

        SetColor(new Color(1, 1, 1, 0));
    }

    private void SetColor(Color color)
    {
        if (_highlighterSpriteRenderer != null) { _highlighterSpriteRenderer.color = color; }

        foreach (Transform child in _highlighter.transform)
        {
            var childColor = new Color(color.r, color.g, color.b, 1);
            _highlighterSpriteRenderer = child.GetComponent<SpriteRenderer>();
            if (_highlighterSpriteRenderer == null) continue;

            child.GetComponent<SpriteRenderer>().color = childColor;
        }
    }

    public override Vector3 GetCellDimensions() { return _dimensions; }

    public void ShowResourceButton()
    {
        if (resourceGameObject == null)
        {
            HideResourceButtons();

            switch (resourceType)
            {
                case ResourceType.Wood:
                    _btnWood.SetActive(true);
                    break;
                case ResourceType.Grain:
                    _btnGrain.SetActive(true);
                    break;
                case ResourceType.Sheep:
                    _btnSheep.SetActive(true);
                    break;
                case ResourceType.Stone:
                    _btnStone.SetActive(true);
                    break;
                case ResourceType.Horse:
                    _btnHorse.SetActive(true);
                    break;
            }
        }
    }

    public void HideResourceButtons()
    {
        _btnWood.SetActive(false);
        _btnGrain.SetActive(false);
        _btnSheep.SetActive(false);
        _btnStone.SetActive(false);
        _btnHorse.SetActive(false);
    }

    public void UpdateResourceIcon()
    {
        if (spriteRendererResource != null)
            spriteRendererResource.gameObject.SetActive(!spriteRendererResource.gameObject.activeSelf);
    }

    public void DestroyResourceBuilding() { Destroy(resourceGameObject); }
}

public enum GroundType
{
    Land,
    Water
};