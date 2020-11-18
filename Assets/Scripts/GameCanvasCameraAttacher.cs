using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvasCameraAttacher : MonoBehaviour
{
    private Canvas _thisCanvas;

    private void Awake()
    {
        _thisCanvas             = GetComponent<Canvas>();

        if (ObjectHolder.Instance != null)
            _thisCanvas.worldCamera = ObjectHolder.Instance.mainCamera;
        else
            _thisCanvas.worldCamera = Camera.main;
    }
}