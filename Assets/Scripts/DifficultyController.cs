using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    private static DifficultyController _instance;
    public static  DifficultyController Instance => _instance;

    public int expansionEventDisplayTurn = 25, maxValue = 3;

    [BoxGroup("Points")]
    public float disableSlimesAndWolvesPoints = .15f;

    private bool areSlimesAndWolvesDisabled;

    [BoxGroup("Slimes & Wolves Caves")]
    [SerializeField] private EnemySpawner[] _enemySpawners;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public void IncreaseEnemyPointsMultiplier(float value)
    {
        WaveSpawner.Instance.enemyPointsMultiplier += value;

        if (WaveSpawner.Instance.enemyPointsMultiplier > maxValue)
            WaveSpawner.Instance.enemyPointsMultiplier = maxValue;
    }

    public void DisableSlimesAndWolves(bool value)
    {
        if (value)
        {
            if (areSlimesAndWolvesDisabled) return;
            
            areSlimesAndWolvesDisabled = true;
        
            WaveSpawner.Instance.enemyPointsMultiplier += disableSlimesAndWolvesPoints;

            if (WaveSpawner.Instance.enemyPointsMultiplier > maxValue)
                WaveSpawner.Instance.enemyPointsMultiplier = maxValue;

            foreach (EnemySpawner e in _enemySpawners)
            {
                e.SetMaxUnitsToSpawn(0);
                e.GetComponent<UnitController>().GetMarker().GetComponent<SpriteRenderer>().enabled = false;
                e.GetComponent<TentEnemyIcon>().DisableIcon();

                foreach (HexagonTile tile in e.GetComponent<Building>().GetBuildingTiles())
                {
                    tile.isSettlementTile = false;
                    tile.settlement = null;
                }
                
                e.GetComponent<Building>().GetBuildingTiles().Clear();
            }
        }
        else
        {
            foreach (EnemySpawner e in _enemySpawners) { e.SetMaxUnitsToSpawn(6); }
        }
    }
}