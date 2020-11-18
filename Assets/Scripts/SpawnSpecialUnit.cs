using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TbsFramework.Cells;
using UnityEngine;

public class SpawnSpecialUnit : MonoBehaviour
{
    [SerializeField]  private Cell _cell;

    private Collider2D _cellCollider;

    [SerializeField] private UnitController _unitToSpawn;

    [SerializeField] private LayerMask _cellLayer;

    [SerializeField] private float _radius = .1f;

    private void Start()
    {
        _cellCollider = Physics2D.OverlapCircle(transform.position, _radius, _cellLayer);

        if (_cellCollider != null) _cell = _cellCollider.GetComponent<Cell>();

        ObjectHolder.Instance.specialUnitSpawners.Add(this);
    }
    
    [Button]
    public void SpawnUnit()
    {
        if (_unitToSpawn != null && _cell != null) UnitSpawner.Instance.SpawnUnitSpecial(_unitToSpawn, _cell);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    [Button]
    public void SpawnEnemyUnit()
    {
        if (_unitToSpawn != null && _cell != null) UnitSpawner.Instance.SpawnUnitSpecial(_unitToSpawn, _cell);
    }
}