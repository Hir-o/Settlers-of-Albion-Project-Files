using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentEnemyIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRendererEnemyIcon;

    [SerializeField]
    private Sprite _sprBear, _sprWolf, _sprWerewolf, _sprSlime, _sprGoblin, _sprOrc, _sprFrogman, _sprTroll, _sprCyclops, _sprYeti;

    public void UpdateEnemyIcon(EnemySpawner.EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemySpawner.EnemyType.Bear:
                _spriteRendererEnemyIcon.sprite = _sprBear;
                break;
            case EnemySpawner.EnemyType.Wolf:
                _spriteRendererEnemyIcon.sprite = _sprWolf;
                break;
            case EnemySpawner.EnemyType.Werewolf:
                _spriteRendererEnemyIcon.sprite = _sprWerewolf;
                break;
            case EnemySpawner.EnemyType.Slime:
                _spriteRendererEnemyIcon.sprite = _sprSlime;
                break;
            case EnemySpawner.EnemyType.Goblin:
                _spriteRendererEnemyIcon.sprite = _sprGoblin;
                break;
            case EnemySpawner.EnemyType.Orc:
                _spriteRendererEnemyIcon.sprite = _sprOrc;
                break;
            case EnemySpawner.EnemyType.Frogman:
                _spriteRendererEnemyIcon.sprite = _sprFrogman;
                break;
            case EnemySpawner.EnemyType.Troll:
                _spriteRendererEnemyIcon.sprite = _sprTroll;
                break;
            case EnemySpawner.EnemyType.Cyclops:
                _spriteRendererEnemyIcon.sprite = _sprCyclops;
                break;
            case EnemySpawner.EnemyType.Yeti:
                _spriteRendererEnemyIcon.sprite = _sprYeti;
                break;
        }
    }

    public void DisableIcon() { _spriteRendererEnemyIcon.sprite = null; }
}