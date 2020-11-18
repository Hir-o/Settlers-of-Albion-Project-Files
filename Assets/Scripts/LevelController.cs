using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private static LevelController _instance;
    public static  LevelController Instance => _instance;

    [Range(1, 3)]
    public int maxUnitUpgradeLevel = 1;

    [Range(1, 4)]
    public int maxSettlementUpgradeLevel = 4;

    [Range(1, 2)]
    public int maxStrongholdUpgradeLevel = 1;

    public bool isSettlerEnabled        = true,
                isClubmanEnabled        = true,
                isPikemanEnabled        = true,
                isArcherEnabled         = true,
                isFootmanEnabled        = true,
                isHussarEnabled         = true,
                isKnightEnabled         = true,
                isArmouredArcherEnabled = true,
                isMountedKnightEnabled  = true,
                isPaladinEnabled        = true,
                isMountedPaladinEnabled = true,
                isKingEnabled           = true,
                isWorkerEnabled         = true;

    private bool isTransitioning;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public void AdvanceToNextLevel()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        
        Transitioner.Instance.GetImageTransition().DOFade(1f, Transitioner.Instance.GetDuration()/2)
                    .SetEase(Transitioner.Instance.GetEaseType()).OnComplete(InitNextLevel);
    }

    public void RestartLevel()
    {
        if (isTransitioning) return;
        
        isTransitioning = true;
        
        Transitioner.Instance.GetImageTransition().DOFade(1f, Transitioner.Instance.GetDuration()/2)
                    .SetEase(Transitioner.Instance.GetEaseType()).OnComplete(InitRestart);
    }

    public void StartLevelAtIndex(int index)
    {
        if (isTransitioning) return;

        isTransitioning = true;
        
        Transitioner.Instance.GetImageTransition().DOFade(1f, Transitioner.Instance.GetDuration() /2)
                    .SetEase(Transitioner.Instance.GetEaseType()).OnComplete(() => InitCustomLevel(index));
    }

    public void InitCustomLevel(int index)
    {
        SceneManager.LoadScene(index, LoadSceneMode.Single);
    }

    private void InitNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    private void InitRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }
}