using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndUI : MonoBehaviour
{
    private static GameEndUI _instance;
    public static  GameEndUI Instance => _instance;

    public GameObject panelGameEnd;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }
}