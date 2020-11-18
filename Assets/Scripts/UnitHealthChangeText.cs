using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class UnitHealthChangeText : MonoBehaviour
{
    private TextMeshPro _tmpHealth;

    [BoxGroup("Colors")]
    [SerializeField] private Color _healthLoseColor, _healthReGainColor;

    [BoxGroup("End String Text")]
    [SerializeField] private string _endString = "Health";

    [BoxGroup("Animation Params")]
    [SerializeField] private float _endXValue, _endScaleValue, _duration = 2.5f;

    [BoxGroup("Animation Params")]
    [SerializeField] private Ease _easeType = Ease.Linear;

    private void Awake() { _tmpHealth = GetComponent<TextMeshPro>(); }

    public void UpdateText(int value, string extraText)
    {
        if (value < 0)
            _tmpHealth.text =
                $"<color=#{ColorUtility.ToHtmlStringRGBA(_healthLoseColor)}> {value}</color> {_endString} {extraText}";
        else if (value > 0)
            _tmpHealth.text =
                $"<color=#{ColorUtility.ToHtmlStringRGBA(_healthReGainColor)}> +{value}</color> {_endString} {extraText}";
        else
            _tmpHealth.text =
                $"<color=#{ColorUtility.ToHtmlStringRGBA(_healthLoseColor)}> -{value}</color> {_endString} {extraText}";

        transform.DOMoveX(transform.position.x + _endXValue, _duration).SetEase(_easeType);
        transform.DOScale(_endScaleValue, _duration - .5f).SetDelay(.5f).SetEase(_easeType);
        _tmpHealth.DOFade(0f, _duration - 1.5f).SetDelay(1.5f).SetEase(_easeType).OnComplete(DestroyText);
    }

    private void DestroyText()
    {
        Destroy(gameObject);
    }
}