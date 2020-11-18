using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NextButtonPulse : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        _rectTransform.DOSizeDelta(new Vector2(72f, 66f), .5f).SetEase(Ease.OutQuad).SetLoops(-1, LoopType.Yoyo);
    }
}