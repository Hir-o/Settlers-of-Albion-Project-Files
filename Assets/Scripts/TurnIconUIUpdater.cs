using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TurnIconUIUpdater : MonoBehaviour
{
    private static TurnIconUIUpdater _instance;
    public static  TurnIconUIUpdater Instance => _instance;

    [SerializeField] private Image  _imgRendererTurnIcon;
    [SerializeField] private Sprite _sprPlayerTurn, _sprEnemyTurn;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() { UpdateTurnIcon(); }

    public void UpdateTurnIcon()
    {
        if (ObjectHolder.Instance.cellGrid.CurrentPlayerNumber == 0)
            _imgRendererTurnIcon.sprite = _sprPlayerTurn;
        else
            _imgRendererTurnIcon.sprite = _sprEnemyTurn;
    }
}