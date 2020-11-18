using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RadiusSpin : MonoBehaviour
{
    private void Start()
    {
        transform.DORotate(new Vector3(0f, 0f,360), 400f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear)
                 .SetLoops(-1, LoopType.Incremental);
    }
}