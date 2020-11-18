using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedButtons : MonoBehaviour
{
    private static SpeedButtons _instance;
    public static  SpeedButtons Instance => _instance;

    [SerializeField] private TextMeshProUGUI _tmpSpeed;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        NormalSpeed();
    }

    public void NormalSpeed()
    {
        if (AITurnSpeedController.Instance != null)
            AITurnSpeedController.Instance.aiTurnSpeed = AITurnSpeedController.AITurnSpeed.Normal;

        _tmpSpeed.text = ">";
    }

    public void FastSpeed()
    {
        if (AITurnSpeedController.Instance != null)
            AITurnSpeedController.Instance.aiTurnSpeed = AITurnSpeedController.AITurnSpeed.Fast;
        
        _tmpSpeed.text = ">>";
    }

    public void ToggleGameSpeed()
    {
        if (AITurnSpeedController.Instance == null) return;

        switch (AITurnSpeedController.Instance.aiTurnSpeed)
        {
            case AITurnSpeedController.AITurnSpeed.Normal:
                _tmpSpeed.text = ">>";
                AITurnSpeedController.Instance.aiTurnSpeed = AITurnSpeedController.AITurnSpeed.Fast;
                break;
            case AITurnSpeedController.AITurnSpeed.Fast: 
                _tmpSpeed.text                             = ">";
                AITurnSpeedController.Instance.aiTurnSpeed = AITurnSpeedController.AITurnSpeed.Normal;

                break;
        }
        
    }
}