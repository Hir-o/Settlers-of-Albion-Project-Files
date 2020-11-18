using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Building _building;
    private TentEnemyIcon _temtEnemyIcon;

    [SerializeField] private int _maxUnitsToSpawn = 6;
    
    public enum EnemyType
    {
        Bear,
        Wolf,
        Werewolf,
        Slime,
        Goblin,
        Orc,
        Frogman,
        Troll,
        Cyclops,
        Yeti
    }

    public EnemyType enemyType = EnemyType.Goblin;

    public UnitController bear, wolf, werewolf, slime, goblin, orc, frogman, troll, cyclops, yeti;

    private int _enemyPoints;

    private UnitController _unitToSpawn;

    private void Awake()
    {
        _building = GetComponent<Building>();
        _temtEnemyIcon = GetComponent<TentEnemyIcon>();

        _temtEnemyIcon.UpdateEnemyIcon(enemyType);
            
        ObjectHolder.Instance.enemySpawners.Add(this);
    }

    [Button]
    public void SpawnUnits()
    {
        int spawnedUnits = 0;
        
        _enemyPoints = (int) WaveSpawner.Instance.totalEnemyPoints;

        foreach (HexagonTile tile in _building.GetBuildingTiles())
        {
            if (tile.IsTaken == false && tile.groundType == GroundType.Land && spawnedUnits < _maxUnitsToSpawn)
            {
                switch (enemyType)
                {
                    case EnemyType.Bear:
                        InitSpawn(bear, WaveSpawner.Instance.wolfCost, tile);
                        spawnedUnits++;
                        break;
                    case EnemyType.Wolf:
                        InitSpawn(wolf, WaveSpawner.Instance.wolfCost, tile);
                        spawnedUnits++;
                        break;
                    case EnemyType.Werewolf:
                        InitSpawn(werewolf, WaveSpawner.Instance.werewolfCost, tile);
                        spawnedUnits++;
                        break;
                    case EnemyType.Slime:
                        InitSpawn(slime, WaveSpawner.Instance.slimeCost, tile);
                        spawnedUnits++;
                        break;
                    case EnemyType.Goblin:
                        InitSpawn(goblin, WaveSpawner.Instance.goblinCost, tile);
                        spawnedUnits++;
                        break;
                    case EnemyType.Orc:
                        InitSpawn(orc, WaveSpawner.Instance.orcCost, tile);
                        spawnedUnits++;
                        break;
                    case EnemyType.Frogman:
                        InitSpawn(frogman, WaveSpawner.Instance.frogmanCost, tile);
                        spawnedUnits++;
                        break;
                    case EnemyType.Troll:
                        InitSpawn(troll, WaveSpawner.Instance.trollCost, tile);
                        spawnedUnits++;
                        break;
                    case EnemyType.Cyclops:
                        InitSpawn(cyclops, WaveSpawner.Instance.cyclopsCost, tile);
                        spawnedUnits++;
                        break;
                    case EnemyType.Yeti:
                        InitSpawn(yeti, WaveSpawner.Instance.yetiCost, tile);
                        spawnedUnits++;
                        break;
                }
            }
        }
    }

    private void InitSpawn(UnitController unit, int cost, HexagonTile tile)
    {
        if (_enemyPoints >= cost * 3 && WaveSpawner.Instance.maxEnemyLevel >= 2)
        {
            _enemyPoints -= cost * 3;
            _unitToSpawn =  UnitSpawner.Instance.SpawnEnemyUnit(unit, tile, 2);
        }
        else if (_enemyPoints >= cost * 2 && WaveSpawner.Instance.maxEnemyLevel >= 1)
        {
            _enemyPoints -= cost * 2;
            _unitToSpawn =  UnitSpawner.Instance.SpawnEnemyUnit(unit, tile, 1);
        }
        else if (_enemyPoints >= cost)
        {
            _enemyPoints -= cost;
            _unitToSpawn = UnitSpawner.Instance.SpawnEnemyUnit(unit, tile, 0);
        }
    }

    public void SetMaxUnitsToSpawn(int value) { _maxUnitsToSpawn = value; }
}