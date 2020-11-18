using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] private Button _btnFinish, _btnRetry;
    [SerializeField] private CanvasGroup _canvasFinish, _canvasRetry;

    [SerializeField] private float _duration = 1f;

    public void Finish()
    {
        _canvasFinish.DOFade(0f, _duration);
        _btnFinish.interactable = false;

        if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
            ReturnToMainMenu();
        else
            LevelController.Instance.AdvanceToNextLevel();
    }

    public void Retry()
    {
        _canvasRetry.DOFade(0f, _duration);
        _btnRetry.interactable = false;
        LevelController.Instance.RestartLevel();
    }

    public void ReturnToMainMenu()
    {
        LevelController.Instance.StartLevelAtIndex(1);
    }
}