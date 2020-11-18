using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurnSpeedController : MonoBehaviour
{
    private static AITurnSpeedController _instance;
    public static  AITurnSpeedController Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public enum AITurnSpeed
    {
        Normal,
        Fast
    }

    public AITurnSpeed aiTurnSpeed = AITurnSpeed.Normal;
}