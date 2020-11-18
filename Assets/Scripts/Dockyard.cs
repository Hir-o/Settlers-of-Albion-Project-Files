using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dockyard : MonoBehaviour
{
    private Collider2D[] _colliders;
    private List<HexagonTile> _tiles = new List<HexagonTile>();

    [SerializeField] private HexagonTile _tile;

    [SerializeField] private LayerMask _tileMask;
    [SerializeField] private float _radius = .1f;

    private void Start() { UpdateTile(); }

    private void UpdateTile()
    {
        _colliders = Physics2D.OverlapCircleAll(transform.position, _radius, _tileMask);

        foreach (Collider2D col in _colliders)
            if (col.GetComponent<HexagonTile>() != null) _tile = col.GetComponent<HexagonTile>();

        if (_tile != null)
        {
            _tile.isDockTile = true;
            _tile.dockyard = this;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
