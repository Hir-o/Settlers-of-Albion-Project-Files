using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementUpgrader : MonoBehaviour
{
    private Building _building;

    [Range(1, 3)]
    [SerializeField] private float _startingLevel;

    private void Start()
    {
        _building = GetComponent<Building>();

//        BuildingUIUpdater.Instance.selectedBuilding = _building;

        for (int i = 0; i < _startingLevel; i++)
            _building.Upgrade();

//        BuildingUIUpdater.Instance.selectedBuilding = null;
    }
}
