using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Transitioner : MonoBehaviour
{
    private static Transitioner _instance;
    public static  Transitioner Instance => _instance;

    [SerializeField] private GameObject _panelTransition;

    private Image _imgTransition;

    [SerializeField] private Ease  _easeType;
    [SerializeField] private float _duration = 2f;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        _imgTransition = _panelTransition.GetComponent<Image>();

        _imgTransition.DOFade(0f, _duration).SetEase(_easeType);
    }

    public Image GetImageTransition() { return _imgTransition; }

    public Ease GetEaseType() { return _easeType; }

    public float GetDuration() { return _duration; }
}