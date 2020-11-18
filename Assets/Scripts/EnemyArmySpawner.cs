using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class EnemyArmySpawner : MonoBehaviour
{
    private static EnemyArmySpawner _instance;
    public static  EnemyArmySpawner Instance => _instance;

    [SerializeField] private int _enemyArmySpawnTurn;
    [SerializeField] private Transform _enemyArmyLocation;

    private bool _hasArmyBeenSpawned;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    [Button]
    public void SpawnEnemyArmy()
    {
        if (ObjectHolder.Instance.cellGrid.turnCount >= _enemyArmySpawnTurn && _hasArmyBeenSpawned == false)
        {
            _hasArmyBeenSpawned = true;
            foreach (var enemySpawner in ObjectHolder.Instance.specialUnitSpawners) enemySpawner.SpawnEnemyUnit();
            
            TBSCamera.Instance.SetMaxYPos(64);
            
            if (_enemyArmyLocation != null)
                TBSCamera.Instance.FocusOn(_enemyArmyLocation);

            if (TutorialUIUpdater.Instance != null)
                TutorialUIUpdater.Instance.OpenPanelTutBattle27();
        }
    }
}