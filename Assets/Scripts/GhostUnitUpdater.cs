using System.Collections;
using System.Collections.Generic;
using TbsFramework.Units;
using UnityEngine;

public class GhostUnitUpdater : MonoBehaviour
{
    [SerializeField] private GameObject[] _gfxUnits;

    [SerializeField] private bool _isSingleEntity;

    private void Start()
    {
        ObjectHolder.Instance.unitGhosts.Add(this);
        
        if (_isSingleEntity) return;

        _gfxUnits[0].SetActive(false);
        _gfxUnits[1].SetActive(false);
        _gfxUnits[2].SetActive(false);

        switch (ObjectHolder.Instance.purchasedUnit.unitType)
        {
            case Unit.UnitType.Pikeman:
                UpdateGFX(UnitLevelController.Instance.pikemanLevel);
                break;
            case Unit.UnitType.Archer:
                UpdateGFX(UnitLevelController.Instance.archerLevel);
                break;
            case Unit.UnitType.Footman:
                UpdateGFX(UnitLevelController.Instance.footmanLevel);
                break;
            case Unit.UnitType.Hussar:
                UpdateGFX(UnitLevelController.Instance.hussarLevel);
                break;
            case Unit.UnitType.Knight:
                UpdateGFX(UnitLevelController.Instance.knightLevel);
                break;
            case Unit.UnitType.ArmouredArcher:
                UpdateGFX(UnitLevelController.Instance.armouredArcherLevel);
                break;
            case Unit.UnitType.MountedKnight:
                UpdateGFX(UnitLevelController.Instance.mountedKnightLevel);
                break;
        }
    }

    private void UpdateGFX(int level)
    {
        for (int i = 0; i < level; i++) _gfxUnits[i].SetActive(true);
    }

    private void OnDestroy()
    {
        ObjectHolder.Instance.unitGhosts.Remove(this);
    }
}