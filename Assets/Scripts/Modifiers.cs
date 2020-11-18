using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifiers : MonoBehaviour
{
    private static Modifiers _instance;
    public static  Modifiers Instance => _instance;

    public int mountainDefenseModifier = 25, mountainAttackModifier = 25;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }
}