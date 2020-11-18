using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Splash : MonoBehaviour
{
    public Image _imgFableLogo;

    public CanvasGroup _cgWarning;

    public Ease _easeType = Ease.OutSine;

    private void Start()
    {
        _imgFableLogo.DOFade(0f, 0f);
        _cgWarning.DOFade(0f, 0f);

        _imgFableLogo.DOFade(1f, 2f).SetEase(_easeType).SetDelay(.5f);

        _imgFableLogo.DOFade(0f, 2f).SetEase(_easeType).SetDelay(3f).OnComplete(ShowWarning);
    }

    private void ShowWarning()
    {
        _cgWarning.DOFade(1f, 2f).SetEase(_easeType);
        _cgWarning.DOFade(0f, 2f).SetEase(_easeType).SetDelay(5f).OnComplete(LoadFirstLevel);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}