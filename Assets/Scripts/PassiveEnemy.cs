using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEnemy : MonoBehaviour
{
    public bool isPassive;

    public LayerMask unitMask;
    public int       radius;

    private Collider2D[] _collider2Ds;

    private UnitController _thisUnit, _unit;

    [SerializeField] private GameObject _radiusPrefab;

    private GameObject _radiusGameObject;

    private void Start()
    {
        _thisUnit = GetComponent<UnitController>();

        if (_radiusPrefab != null && isPassive)
        {
            if (Quests.Instance.mainQuestType == Quests.MainQuestType.Development)
            {
                _radiusGameObject =
                    Instantiate(_radiusPrefab, transform.position, Quaternion.identity);
                _radiusGameObject.transform.parent = transform;
            }
        }
    }

    public void CheckFoEnemiesInRange()
    {
        if (isPassive)
        {
            _collider2Ds = Physics2D.OverlapCircleAll(transform.position, radius, unitMask);

            foreach (Collider2D col in _collider2Ds)
            {
                _unit = col.GetComponent<UnitController>();

                if (_unit != null && _unit.PlayerNumber == 0)
                {
                    isPassive = false;
                    _thisUnit.OnTurnStart();
                    
                    if (_radiusGameObject != null)
                        _radiusGameObject.SetActive(false);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}