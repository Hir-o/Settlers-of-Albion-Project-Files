using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPassiveDisabler : MonoBehaviour
{
    public int turnsUntilBecomingActive = 5;

    private PassiveEnemy _passiveEnemy;
    private UnitController _unitController;

    private void Awake()
    {
        _passiveEnemy = GetComponent<PassiveEnemy>();
        _unitController = GetComponent<UnitController>();

        _unitController.enemyPassiveDisabler = this;
    }

    public void ActivateEnemy() { _passiveEnemy.isPassive = false; }
}
