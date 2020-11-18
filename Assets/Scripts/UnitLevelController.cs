using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class UnitLevelController : MonoBehaviour
{
    private static UnitLevelController _instance;
    public static  UnitLevelController Instance => _instance;
    
    public int pikemanLevel        = 1,
               archerLevel         = 1,
               footmanLevel        = 1,
               hussarLevel         = 1,
               knightLevel         = 1,
               armouredArcherLevel = 1,
               mountedKnightLevel  = 1;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }
}