using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TbsFramework.Cells;
using UnityEngine;

public class TileSpriteRandomizer : MonoBehaviour
{
    [BoxGroup("SpriteRenderer to randomize")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [BoxGroup("Tile Sprites")]
    [SerializeField] private Sprite[] _sprites;

    private HexagonTile _hexagonTile;

    private int _randomIndex, _randomWaveChance, _waveChance = 5, _randomFlipChance, _flipChance = 5;

    private void Awake()
    {
        _hexagonTile = GetComponent<HexagonTile>();

        if (_hexagonTile.groundType == GroundType.Water)
        {
            _randomWaveChance = Random.Range(0, 10);
            if (_randomWaveChance <= _waveChance)
            {
                _spriteRenderer.sprite = null;
                return;
            }
        }

        if (_spriteRenderer != null)
        {
            _randomIndex = Random.Range(0, _sprites.Length);
            _spriteRenderer.sprite = _sprites[_randomIndex];
            
            _randomFlipChance = Random.Range(0, 10);
            if (_randomFlipChance <= _flipChance) _spriteRenderer.flipX = true;
        }
    }
}
