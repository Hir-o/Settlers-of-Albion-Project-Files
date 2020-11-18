using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TbsFramework.Cells;
using TbsFramework.Grid;
using TbsFramework.Grid.GridStates;
using TbsFramework.Units;
using TbsFramework.Units.UnitStates;
using UnityEngine;

public class UnitController : Unit
{
    [BoxGroup("Highlight Tile")]
    [SerializeField] private SpriteRenderer _highlightTile;

    [BoxGroup("Unit Spriterenderers")]
    [SerializeField] private SpriteRenderer _unitSpriteRenderer, _markerSpriteRenderer;

    [BoxGroup("Select Sprite Color")]
    [SerializeField] private Color _markAsFriendlyColor,
                                   _markAsReachableEnemyColor,
                                   _markAsSelectedColor,
                                   _markAsFinishedColor,
                                   _markAsFinishedUnitColor;

    [BoxGroup("Marker Gameobject")]
    [SerializeField] private GameObject _marker;

    [BoxGroup("Marker Sprite Color")]
    [SerializeField] private Color _markerNormalColor, _markerSelectedColor, _markerFinishedColor;

    [BoxGroup("Marker Ease Type")]
    [SerializeField] private Ease _ease = Ease.Flash;

    [BoxGroup("Unit GFX Holder")]
    [SerializeField] private GameObject _gfxHolder;

    [BoxGroup("Description")]
    [SerializeField] private string _name, _description;

    [BoxGroup("Upkeep")]
    public int upkeepGold, upkeepGrain, upkeepSheep, upkeepHorse;

    [BoxGroup("Healing Percentage")]
    [SerializeField] private int _healingPercentage;

    [SerializeField] private enum Facing
    {
        Left,
        Right
    }

    [SerializeField] public enum Level
    {
        Level1,
        Level2,
        Level3
    }

    [SerializeField] private Facing _facing = Facing.Right;

    public Level _level = Level.Level1;

    public bool skipThisTurn;

    [HideInInspector]
    public EnemyPassiveDisabler enemyPassiveDisabler;

    private Coroutine    _pulseCoroutine;
    private Healthbar    _healthbar;
    private Building     _building;
    private HexagonTile  _tile;
    private PassiveEnemy _passiveEnemy;

    [SerializeField] private Transform[]      _unitGfx;
    private                  SpriteRenderer[] _unitSpriteRenderers;

    private bool _isFinished, _isHealing, _isDisbanding, _hasSupplies;

    private float _unitGfxHolderInitPosX, _markerGfxInitPosX, _markerSpriteInitScale;

    public override void Initialize()
    {
        base.Initialize();
        transform.localPosition += new Vector3(0, 0, -1);

        _unitGfx             = _gfxHolder.GetComponentsInChildren<Transform>(true);
        _unitSpriteRenderers = _gfxHolder.GetComponentsInChildren<SpriteRenderer>(true);

        _markerGfxInitPosX     = _marker.transform.position.x;
        _unitGfxHolderInitPosX = _gfxHolder.transform.position.x;
        _markerSpriteInitScale = _markerSpriteRenderer.gameObject.transform.localScale.x;

        HitPoints = MaxHitPoints;

        UpdateUnitLevel();

        if (_healthbar != null)
        {
            _healthbar.UpdateMountainBonusIcon(Cell as HexagonTile);

            if (unitType == UnitType.Building) _healthbar.SetRepairHammerIcon();
        }

        if (PlayerNumber == 0)
        {
            switch (unitType)
            {
                case UnitType.Villager:
                    Statistics.Instance.spawnedVillagers++;
                    break;
                case UnitType.Pikeman:
                    Statistics.Instance.spawnedPikemans++;
                    break;
                case UnitType.Archer:
                    Statistics.Instance.spawnedArchers++;
                    break;
                case UnitType.Footman:
                    Statistics.Instance.spawnedFootmans++;
                    break;
                case UnitType.Hussar:
                    Statistics.Instance.spawnedHussars++;
                    break;
                case UnitType.Knight:
                    Statistics.Instance.spawnedKnights++;
                    break;
                case UnitType.ArmouredArcher:
                    Statistics.Instance.spawnedArmouredArchers++;
                    break;
                case UnitType.MountedKnight:
                    Statistics.Instance.spawnedMountedKnights++;
                    break;
                case UnitType.Paladin:
                    Statistics.Instance.spawnedPaladins++;
                    break;
                case UnitType.MountedPaladin:
                    Statistics.Instance.spawnedMountedPaladins++;
                    break;
                case UnitType.King:
                    Statistics.Instance.spawnedKings++;
                    break;
            }
        }

        Quests.Instance.CheckOptionalQuests();
    }

    private IEnumerator DeleteUnit() { yield return new WaitForSeconds(.2f); }

    public override void OnTurnStart()
    {
        base.OnTurnStart();

        if (PlayerNumber == 0) SetMarkerColor(_markerNormalColor); //reset markers for the player only

        SetColor(Color.white);

        if (HitPoints >= MaxHitPoints && _isHealing) SetIsHealing(false);

        if (_isDisbanding) DefendHandler(this, MaxHitPoints * 2, false, true, "");

        if (unitType == UnitType.Building && _building != null) { _building.CheckForSiegingEnemies(); }

        CheckForSupplies(false);

        if (_healthbar != null) _healthbar.UpdateNoSuppliesIcon(!_hasSupplies);

//        if (_isHealing && _hasSupplies) Heal();

        if (PlayerNumber   == 0 && HitPoints < MaxHitPoints && _isHealing && _hasSupplies && _isDisbanding == false &&
            _tile.isHazard == false)
            Heal();

        if (_healthbar != null) _healthbar.UpdateMountainBonusIcon(Cell as HexagonTile);

        if (PlayerNumber != 0)
        {
            if (_passiveEnemy == null) _passiveEnemy = GetComponent<PassiveEnemy>();

            if (_passiveEnemy != null)
            {
                if (_passiveEnemy.isPassive)
                    ActionPoints = 0;
                else
                    ActionPoints = 1;

                _passiveEnemy.CheckFoEnemiesInRange();
            }
        }
    }

    public override void OnTurnEnd()
    {
        base.OnTurnEnd();

        _tile = (HexagonTile) Cell;

        if (_tile.isHazard && PlayerNumber == 0) TakeAttritionDamage();

        _isFinished  = false;
        skipThisTurn = false;

        CheckForSupplies(true);

        if (_healthbar != null) _healthbar.UpdateNoSuppliesIcon(!_hasSupplies);

        if (PlayerNumber == 0)
        {
            if (HitPoints      < MaxHitPoints && ActionPoints > 0 && unitType != UnitType.Building && _hasSupplies && _isDisbanding == false &&
                _tile.isHazard == false)
                SetIsHealing(true);
        }
    }

    public void CheckForSupplies(bool takeDamage)
    {
        if (PlayerNumber != 0) return;

        _hasSupplies = true;

        if (upkeepGold                                > 0 && ResourcesController.Instance.gold <= 0 &&
            ResourcesController.Instance.isMakingGold == false)
        {
            _hasSupplies = false;
            if (takeDamage) StartCoroutine(TakeNoSuppliesDamage(1f));
            return;
        }

        if (upkeepGrain                                > 0 && ResourcesController.Instance.grain <= 0 &&
            ResourcesController.Instance.isMakingGrain == false)
        {
            _hasSupplies = false;
            if (takeDamage) StartCoroutine(TakeNoSuppliesDamage(1f));
            return;
        }

        if (upkeepSheep                                > 0 && ResourcesController.Instance.sheep <= 0 &&
            ResourcesController.Instance.isMakingSheep == false)
        {
            _hasSupplies = false;
            if (takeDamage) StartCoroutine(TakeNoSuppliesDamage(1f));
            return;
        }

        if (upkeepHorse                                > 0 && ResourcesController.Instance.horse <= 0 &&
            ResourcesController.Instance.isMakingHorse == false)
        {
            _hasSupplies = false;
            if (takeDamage) StartCoroutine(TakeNoSuppliesDamage(1f));
        }

        if (_healthbar != null) _healthbar.UpdateNoSuppliesIcon(!_hasSupplies);
    }

    private void TakeAttritionDamage()
    {
        float attritionDamage = (MaxHitPoints / 100f) * 40f;
        DefendHandler(this, Mathf.RoundToInt(attritionDamage), true, true, "Attrition");
    }

    private IEnumerator TakeNoSuppliesDamage(float delay)
    {
        yield return new WaitForSeconds(delay);

        float attritionDamage = (MaxHitPoints / 100f) * 10f;
        DefendHandler(this, Mathf.RoundToInt(attritionDamage), true, true, "No Supplies");
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        ResourcesController.Instance.UpdateOnlyResourceUIText();
    }

    public override bool IsCellMovableTo(Cell cell)
    {
        return base.IsCellMovableTo(cell) && (cell as HexagonTile).groundType != GroundType.Water;
        //Prohibits moving to cells that are marked as water.
    }

    public override bool IsCellTraversable(Cell cell)
    {
        return base.IsCellTraversable(cell) && (cell as HexagonTile).groundType != GroundType.Water;
        //Prohibits moving through cells that are marked as water.
    }

    public override void Move(Cell destinationCell, List<Cell> path)
    {
        UpdateUnitDirection(destinationCell.transform);
        base.Move(destinationCell, path);

        _healthbar.UpdateMountainBonusIcon(destinationCell as HexagonTile);

        if (PlayerNumber == 0) _healthbar.UpdateAttritionIcon(destinationCell as HexagonTile);

        if (PlayerNumber != 0)
        {
            if (AITurnSpeedController.Instance != null && AITurnSpeedController.Instance.aiTurnSpeed ==
                AITurnSpeedController.AITurnSpeed.Fast)
                return;
        }

        AudioController.Instance.SFXSteps();
    }

    public override void OnUnitSelected()
    {
        base.OnUnitSelected();

        if (unitType != UnitType.Building)
        {
            UnitUIUpdater.Instance.UpdateUnitPanel(this, unitType, _name, _description, _level, _unitSpriteRenderers);
            EndTurnUIUpdater.Instance.currentSelectedUnit = this;
            EndTurnUIUpdater.Instance.UpdateSkipButton();
        }
        else
        {
            if (_building.isSettlement) _building.ShowBuildButtons();

            BuildingUIUpdater.Instance.UpdateBuildingPanel(_building);
        }

        if (ObjectHolder.Instance.purchasedUnit != null)
        {
            ObjectHolder.Instance.purchasedUnit = null;
            UnitSpawner.Instance.ResetHighlightedTiles();
        }

//        AudioController.Instance.SFXSelectUnit();
    }

    public override void OnUnitDeselected()
    {
        base.OnUnitDeselected();

        if (unitType == UnitType.Building)
        {
            SetState(new UnitStateNormal(this));
            _building.HideBuildButtons();
            BuildingUIUpdater.Instance.panelBuilding.SetActive(false);
        }

        if (unitType == UnitType.Settler) SetState(new UnitStateNormal(this));

        if (_isFinished == false) SetMarkerColor(_markerNormalColor);

        UnMark();

        if (unitType != UnitType.Building)
        {
            StopUnitAnimation();
            UnitUIUpdater.Instance.panelUnit.SetActive(false);
            EndTurnUIUpdater.Instance.currentSelectedUnit = null;
            EndTurnUIUpdater.Instance.UpdateSkipButton();
        }
    }

    public Building GetBuilding()                  { return _building; }
    public void     SetBuilding(Building building) { _building = building; }

    public override void MarkAsAttacking(Unit other)
    {
        switch (unitType)
        {
            case UnitType.Villager:
                AudioController.Instance.SFXMeleeAttack();
                break;
            case UnitType.Pikeman:
                AudioController.Instance.SFXMeleeAttack();
                break;
            case UnitType.Archer:
                AudioController.Instance.SFXShootArrow();
                break;
            case UnitType.Footman:
                AudioController.Instance.SFXMeleeAttack();
                break;
            case UnitType.Hussar:
                AudioController.Instance.SFXMeleeAttack();
                break;
            case UnitType.Knight:
                AudioController.Instance.SFXMeleeAttack();
                break;
            case UnitType.ArmouredArcher:
                AudioController.Instance.SFXShootArrow();
                break;
            case UnitType.MountedKnight:
                AudioController.Instance.SFXMeleeAttack();
                break;
            case UnitType.Paladin:
                AudioController.Instance.SFXHammerAttack();
                break;
            case UnitType.MountedPaladin:
                AudioController.Instance.SFXHammerAttack();
                break;
            case UnitType.King:
                AudioController.Instance.SFXHammerAttack();
                break;
            case UnitType.Building:
                AudioController.Instance.SFXShootArrow();
                break;
        }

        if (unitType == UnitType.Building) return;

        if (other != null) UpdateUnitDirection(other.transform);

        if (other != null) StartCoroutine(Jerk(other, 0.2f));

        if (_isHealing) SetIsHealing(false);

        UnitUIUpdater.Instance.UpdateUnitPanel(this, unitType, _name, _description, _level, _unitSpriteRenderers);
    }

    public override void MarkAsDefending(Unit other)
    {
        if (other != null) UpdateUnitDirection(other.transform);

        StartCoroutine(Glow(new Color(1, 0.5f, 0.5f), 1));

        if (_isHealing) SetIsHealing(false);
    }

    public override void MarkAsDestroyed() { }

    private IEnumerator Jerk(Unit other, float movementTime)
    {
        var   heading   = other.transform.position - transform.position;
        var   direction = heading / heading.magnitude;
        float startTime = Time.time;

        while (true)
        {
            var currentTime = Time.time;
            if (startTime + movementTime < currentTime) break;
            transform.position = Vector3.Lerp(transform.position,
                                              transform.position + (direction / 1.25f),
                                              ((startTime + movementTime) - currentTime) / 20f);
            yield return 0;
        }

        startTime = Time.time;
        while (true)
        {
            var currentTime = Time.time;
            if (startTime + movementTime < currentTime) break;
            transform.position = Vector3.Lerp(transform.position,
                                              transform.position - (direction / 1.25f),
                                              ((startTime + movementTime) - currentTime) / 20f);
            yield return 0;
        }

        transform.position = Cell.transform.position + new Vector3(0, 0, -1);
    }

    private IEnumerator Glow(Color color, float cooloutTime)
    {
        float startTime = Time.time;

        while (true)
        {
            var currentTime = Time.time;
            if (startTime + cooloutTime < currentTime) break;

            foreach (SpriteRenderer _unitSprite in _unitSpriteRenderers)
            {
                _unitSprite.color = Color.Lerp(Color.white, color, (startTime + cooloutTime) - currentTime);
            }

            yield return 0;
        }

        foreach (SpriteRenderer _unitSprite in _unitSpriteRenderers) { _unitSprite.color = Color.white; }
    }

    private IEnumerator Pulse(float breakTime, float delay, float scaleFactor)
    {
        var baseScale = Vector3.one;
        while (true)
        {
            float time1 = Time.time;
            while (time1 + delay > Time.time)
            {
                transform.localScale = Vector3.Lerp(baseScale * scaleFactor, baseScale, (time1 + delay) - Time.time);
                yield return 0;
            }

            yield return new WaitForSeconds(breakTime);

            float time2 = Time.time;
            while (time2 + delay > Time.time)
            {
                transform.localScale = Vector3.Lerp(baseScale, baseScale * scaleFactor, (time2 + delay) - Time.time);
                yield return 0;
            }
        }
    }

    private void Heal()
    {
        float hitPointsToHeal    = (MaxHitPoints / 100f) * _healingPercentage;
        int   hitPointsAfterHeal = HitPoints + (int) hitPointsToHeal;

        if (hitPointsAfterHeal > MaxHitPoints)
        {
            hitPointsToHeal    = hitPointsToHeal - (hitPointsAfterHeal - MaxHitPoints);
            hitPointsAfterHeal = MaxHitPoints;

            SetIsHealing(false);
        }

        HitPoints = hitPointsAfterHeal;

        Instantiate(ObjectHolder.Instance.unitHealthText, transform.position, Quaternion.Euler(0f, 0f, -90f))
            .UpdateText((int) hitPointsToHeal, "");

        StartCoroutine(Glow(new Color(0.937f, 0.984f, 0.003f), 1));
        UpdateHealthbar();
    }

    public void Disband() { SetIsDisbanding(!_isDisbanding); }

    public void FormSettlement(bool isOutpost)
    {
        HexagonTile tile = (HexagonTile) Cell;

        if (tile.resourceGameObject != null) return;

        UnitController building;

        var cellGrid = ObjectHolder.Instance.cellGrid;

        if (isOutpost)
        {
            building =
                Instantiate(ObjectHolder.Instance.outpost, transform.position, Quaternion.Euler(0f, 0f, -90f))
                    .GetComponent<UnitController>();
        }
        else
        {
            building =
                Instantiate(ObjectHolder.Instance.settlement, transform.position, Quaternion.Euler(0f, 0f, -90f))
                    .GetComponent<UnitController>();
        }

        building.Cell             = Cell;
        building.Cell.CurrentUnit = building;
        building.Initialize();
        building.transform.SetParent(transform.parent);

        cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
        cellGrid.AddUnit(building.GetComponent<Transform>());

        EndTurnUIUpdater.Instance.UpdateEndTurnButton();

        ResourcesController.Instance.UpdateOnlyResourceUIText();

        StartCoroutine(ClickBuilding(building));

        AudioController.Instance.SFXFormSettlementOrOutpost();
    }

    private IEnumerator ClickBuilding(UnitController building)
    {
        yield return new WaitForSeconds(.001f);
        ObjectHolder.Instance.cellGrid.CellGridState.OnUnitClicked(building);
        ObjectHolder.Instance.cellGrid.CellGridState =
            new CellGridStateUnitSelected(ObjectHolder.Instance.cellGrid, building.GetComponent<Unit>());

        DefendHandler(this, MaxHitPoints * 2, false, true, "");

        Cell.IsTaken = true;
    }

    public override void UpdateHealthbar()
    {
        if (_healthbar != null) _healthbar.SetSize((float) HitPoints / MaxHitPoints);
    }

    [Button]
    private void InitializeHeal() { SetIsHealing(true); } //todo delete

    public void SetIsHealing(bool value)
    {
        if (unitType != UnitType.Building)
        {
            if (value)
            {
                if (ActionPoints > 0)
                {
                    if (HitPoints < MaxHitPoints) _isHealing = value;

                    _healthbar.UpdateHealIcon(_isHealing);
                }
            }
            else
            {
                _isHealing = false;
                _healthbar.UpdateHealIcon(_isHealing);
            }
        }
        else
        {
            _isHealing = value;
            _healthbar.UpdateHealIcon(_isHealing);
        }
    }

    public void SetIsDisbanding(bool value)
    {
        _isDisbanding = value;
        _healthbar.UpdateDisbandIcon(_isDisbanding);

        EndTurnUIUpdater.Instance.UpdateEndTurnButton();
    }

    public bool GetIsDisbanding() { return _isDisbanding; }

    public override void MarkAsFriendly()
    {
        //SetHighlighterColor(_markAsFriendlyColor);
    }

    public override void MarkAsReachableEnemy() { SetHighlighterColor(_markAsReachableEnemyColor); }

    public override void MarkAsSelected()
    {
        if (unitType != UnitType.Building) _pulseCoroutine = StartCoroutine(Pulse(.2f, 0.65f, 1.1f));

        SetHighlighterColor(_markAsSelectedColor);
        SetMarkerColor(_markerSelectedColor);

        _markerSpriteRenderer.transform.DOScale(1f, 0f);
        _markerSpriteRenderer.transform.DOScale(_markerSpriteInitScale + .3f, .85f).SetEase(_ease)
                             .SetLoops(-1, LoopType.Yoyo);
    }

    public override void MarkAsFinished()
    {
        if (unitType == UnitType.Building || unitType == UnitType.Settler) return;

        SetColor(_markAsFinishedUnitColor);
        SetHighlighterColor(_markAsFinishedColor);
        SetMarkerColor(_markerFinishedColor);

        StopUnitAnimation();

        DOTween.Kill(_markerSpriteRenderer.transform);
        _markerSpriteRenderer.transform.DOScale(1f, .5f).SetEase(_ease);

        _isFinished = true;

        EndTurnUIUpdater.Instance.UpdateEndTurnButton();
        EndTurnUIUpdater.Instance.UpdateSkipButton();
    }

    public override void UnMark()
    {
        SetHighlighterColor(Color.clear);

        if (_markerSpriteRenderer != null)
        {
            DOTween.Kill(_markerSpriteRenderer.transform);
            _markerSpriteRenderer.transform.DOScale(_markerSpriteInitScale, .5f).SetEase(_ease);
        }
    }

    public bool         GetIsFinished()   { return _isFinished; }
    public Healthbar    GetHealthbar()    { return _healthbar; }
    public string       GetName()         { return _name; }
    public string       GetDescription()  { return _description; }
    public bool         GetHasSupplies()  { return _hasSupplies; }
    public PassiveEnemy GetPassiveEnemy() { return _passiveEnemy; }
    public GameObject   GetMarker()       { return _marker; }

    private void SetColor(Color color)
    {
        if (PlayerNumber != 0) return;

        if (_unitSpriteRenderers.Length > 0)
            foreach (SpriteRenderer _unitSprite in _unitSpriteRenderers)
                _unitSprite.color = color;
    }

    private void SetHighlighterColor(Color color)
    {
        if (_highlightTile != null) { _highlightTile.color = color; }
    }

    private void SetMarkerColor(Color color)
    {
        if (PlayerNumber          != 0) return;
        if (_markerSpriteRenderer != null) { _markerSpriteRenderer.color = color; }
    }

    public void SetHealthbar(Healthbar newHealthbar) { _healthbar = newHealthbar; }

    private void UpdateUnitDirection(Transform destination)
    {
        if (unitType != UnitType.Building)
        {
            if (destination.position.y > transform.position.y && _facing == Facing.Right)
            {
                for (int i = 0; i < _unitSpriteRenderers.Length; i++) _unitSpriteRenderers[i].flipX = false;
                _facing = Facing.Left;
            }
            else if (destination.position.y < transform.position.y && _facing == Facing.Left)
            {
                for (int i = 0; i < _unitSpriteRenderers.Length; i++) _unitSpriteRenderers[i].flipX = true;
                _facing = Facing.Right;
            }
        }
    }

    [Button]
    public void UpdateUnitLevel()
    {
        if (PlayerNumber == 0)
        {
            switch (unitType)
            {
                case UnitType.Pikeman:
                    _level = (Level) UnitLevelController.Instance.pikemanLevel - 1;
                    break;
                case UnitType.Archer:
                    _level = (Level) UnitLevelController.Instance.archerLevel - 1;
                    break;
                case UnitType.Footman:
                    _level = (Level) UnitLevelController.Instance.footmanLevel - 1;
                    break;
                case UnitType.Hussar:
                    _level = (Level) UnitLevelController.Instance.hussarLevel - 1;
                    break;
                case UnitType.Knight:
                    _level = (Level) UnitLevelController.Instance.knightLevel - 1;
                    break;
                case UnitType.ArmouredArcher:
                    _level = (Level) UnitLevelController.Instance.armouredArcherLevel - 1;
                    break;
                case UnitType.MountedKnight:
                    _level = (Level) UnitLevelController.Instance.mountedKnightLevel - 1;
                    break;
            }
        }

        if (unitType != UnitType.Building)
        {
            UpdateUnitStats();
            UpdateUnitGFX();
        }
    }

    private void UpdateUnitStats()
    {
        int hitPointsDifference = MaxHitPoints - HitPoints;

        switch (_level)
        {
            case Level.Level2:
                MaxHitPoints = InitMaxHitPoints * 2;
                HitPoints    = MaxHitPoints - hitPointsDifference;
                UpdateHealthbar();
                break;
            case Level.Level3:
                MaxHitPoints = InitMaxHitPoints * 3;
                HitPoints    = MaxHitPoints - hitPointsDifference;
                UpdateHealthbar();
                break;
        }
    }

    private void UpdateUnitGFX()
    {
        if (_unitGfx.Length > 2) // because GetComponentsInChildren includes parent
        {
            switch (_level)
            {
                case Level.Level2:
                    for (int i = 0; i < _unitGfx.Length - 1; i++) _unitGfx[i].gameObject.SetActive(true);
                    break;
                case Level.Level3:
                    for (int i = 0; i < _unitGfx.Length; i++) _unitGfx[i].gameObject.SetActive(true);
                    break;
            }
        }
    }

    private void StopUnitAnimation()
    {
        if (_pulseCoroutine != null) StopCoroutine(_pulseCoroutine);

        transform.DOScale(1f, .3f);
    }

    private void OnDestroy()
    {
        _unitSpriteRenderers = new SpriteRenderer[0];

        if (Statistics.Instance != null)
            switch (unitType)
            {
                case UnitType.Villager:
                    Statistics.Instance.spawnedVillagers--;
                    break;
                case UnitType.Pikeman:
                    Statistics.Instance.spawnedPikemans--;
                    break;
                case UnitType.Archer:
                    Statistics.Instance.spawnedArchers--;
                    break;
                case UnitType.Footman:
                    Statistics.Instance.spawnedFootmans--;
                    break;
                case UnitType.Hussar:
                    Statistics.Instance.spawnedHussars--;
                    break;
                case UnitType.Knight:
                    Statistics.Instance.spawnedKnights--;
                    break;
                case UnitType.ArmouredArcher:
                    Statistics.Instance.spawnedArmouredArchers--;
                    break;
                case UnitType.MountedKnight:
                    Statistics.Instance.spawnedMountedKnights--;
                    break;
                case UnitType.Paladin:
                    Statistics.Instance.spawnedPaladins--;
                    break;
                case UnitType.MountedPaladin:
                    Statistics.Instance.spawnedMountedPaladins--;
                    break;
                case UnitType.King:
                    Statistics.Instance.spawnedKings--;
                    break;
            }

        if (Quests.Instance != null)
        {
            Quests.Instance.CheckOptionalQuests();
            Quests.Instance.CheckLoseConditions();

            if (PlayerNumber != 0 && Quests.Instance.mainQuestType != Quests.MainQuestType.Development)
                Quests.Instance.CheckWinConditions();
        }
    }
}