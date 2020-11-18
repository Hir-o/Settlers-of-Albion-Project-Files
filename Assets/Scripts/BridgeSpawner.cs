using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _bridge;

    private HexagonTile _hexagonTile;

    private void Awake()
    {
        _hexagonTile = GetComponent<HexagonTile>();
        
        if (_bridge != null)
            _bridge.SetActive(false);
        
        ObjectHolder.Instance.bridgeSpawners.Add(this);
    }
    
    public void ActivateBridge()
    {
        _hexagonTile.groundType = GroundType.Land;
        
        if (_bridge != null)
            _bridge.SetActive(true);
    }
}
