using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestUIUpdater : MonoBehaviour
{
    private static QuestUIUpdater _instance;
    public static  QuestUIUpdater Instance => _instance;

    [SerializeField] private GameObject _questsPanel, _optionalTextGameObject, _optionalLineGameObject;

    [BoxGroup("Main Quest")]
    [SerializeField] private TextMeshProUGUI _tmpMainQuest;

    [BoxGroup("Main Quest")]
    [SerializeField] private Image _imgMainQuestState;

    [BoxGroup("Optional")]
    [SerializeField] private TextMeshProUGUI _tmpOptionalQuest1, _tmpOptionalQuest2, _tmpOptionalQuest3;

    [BoxGroup("Optional")]
    [SerializeField] private Image _imgOptionalQuestState1, _imgOptionalQuestState2, _imgOptionalQuestState3;

    [BoxGroup("Quest State Images")]
    [SerializeField] private Sprite _spriteNormalState, _spriteFinishedState, _spriteFailedState;

    [BoxGroup("Next Wave Text")]
    [SerializeField] private TextMeshProUGUI _tmpNextWaveIn;

    [BoxGroup("Finish Panel")]
    [SerializeField] private GameObject _finishPanel, _retryPanel;

    public  Color green, red;
    private Color _originalTextColor;

    private int devPoints;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        _originalTextColor = _tmpMainQuest.color;

        UpdateMainQuest();
        UpdateSideQuests();
        UpdateQuestPanelSize();
    }

    private void Start() { UpdateNextWaveText(); }

    public void UpdateNextWaveText()
    {
        switch (Quests.Instance.mainQuestType)
        {
            case Quests.MainQuestType.Development:
                if (WaveSpawner.Instance.turnsToNextWave - 1 == 1)
                    _tmpNextWaveIn.text =
                        $"Next Wave in: {WaveSpawner.Instance.turnsToNextWave - 1} Turn";
                else
                    _tmpNextWaveIn.text =
                        $"Next Wave in: {WaveSpawner.Instance.turnsToNextWave - 1} Turns";
                break;
            case Quests.MainQuestType.Defend:
                if (Quests.Instance.reinforcementsAfterWave <= 0)
                    _tmpNextWaveIn.text =
                        $"The Army Has Joined";
                else if (Quests.Instance.reinforcementsAfterWave == 1)
                    _tmpNextWaveIn.text =
                        $"Army Arriving in: {Quests.Instance.reinforcementsAfterWave} Turn";
                else
                    _tmpNextWaveIn.text =
                        $"Army Arriving in: {Quests.Instance.reinforcementsAfterWave} Turns";
                break;
        }
    }

    public void UpdateMainQuest()
    {
        switch (Quests.Instance.mainQuestType)
        {
            case Quests.MainQuestType.Development:
                if (devPoints < Quests.Instance.currentDevelopmentPoints)
                {
                    _tmpMainQuest.color = green;
                    _tmpMainQuest.DOColor(_originalTextColor, 1f).SetEase(Ease.InOutQuad);
                }
                else if (devPoints > Quests.Instance.currentDevelopmentPoints)
                {
                    _tmpMainQuest.color = red;
                    _tmpMainQuest.DOColor(_originalTextColor, 1f).SetEase(Ease.InOutQuad);
                }

                if (Quests.Instance.currentDevelopmentPoints >= Quests.Instance.developmentPointsToWin)
                {
                    _imgMainQuestState.sprite = _spriteFinishedState;
                    _tmpMainQuest.fontStyle   = FontStyles.Underline;
                }

                devPoints = Quests.Instance.currentDevelopmentPoints;

                _tmpMainQuest.text =
                    $"Have {Quests.Instance.developmentPointsToWin} Victory Points\n" +
                    $"(Current: {Quests.Instance.currentDevelopmentPoints}\\{Quests.Instance.developmentPointsToWin})";
                break;
            case Quests.MainQuestType.Battle:
                _tmpMainQuest.text =
                    $"Defeat the enemy army";
                break;
            case Quests.MainQuestType.Defend:
                if (Quests.Instance.reinforcementsAfterWave > 0)
                    _tmpMainQuest.text =
                        $"Defend your settlements until your main army arrives.";
                else
                    _tmpMainQuest.text =
                        $"Defeat all the remaining enemies";

                break;
        }
    }

    [Button]
    public void FinishGame() { ShowFinishPanel(true); }

    public void ShowFinishPanel(bool isWon)
    {
        if (isWon)
        {
            if (_finishPanel           != null && _finishPanel.activeSelf == false && _retryPanel != null &&
                _retryPanel.activeSelf == false)
            {
                _finishPanel.GetComponent<CanvasGroup>().alpha = 0f;
                _finishPanel.GetComponent<CanvasGroup>().DOFade(1f, 1f);
                _finishPanel.SetActive(true);

                if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1)
                {
                    if (GameEndUI.Instance != null && GameEndUI.Instance.panelGameEnd != null)
                        GameEndUI.Instance.panelGameEnd.SetActive(true);
                }

                if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
                    SaveController.Instance.SaveLevelIndex(SceneManager.GetActiveScene().buildIndex);

                AudioController.Instance.SFXLevelWin();
            }
        }
        else
        {
            if (_retryPanel != null && _finishPanel.activeSelf == false)
            {
                _retryPanel.GetComponent<CanvasGroup>().alpha = 0f;
                _retryPanel.GetComponent<CanvasGroup>().DOFade(1f, 1f);
                _retryPanel.SetActive(true);

                AudioController.Instance.SFXLevelLose();
            }
        }
    }

    private void UpdateQuestPanelSize()
    {
        if (Quests.Instance.mainQuestType == Quests.MainQuestType.Battle)
        {
            _optionalTextGameObject.SetActive(false);
            _optionalLineGameObject.SetActive(false);

            _questsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(383, 137);
        }
    }

    public void UpdateSideQuests()
    {
        if (Quests.Instance.sideQuests[0].isEnabled)
        {
            _tmpOptionalQuest1.text =
                $"{Quests.Instance.sideQuests[0].sideQuestText}" +
                $" ({Quests.Instance.sideQuests[0].goldReward}<sprite=0 index=4>)";

            if (Quests.Instance.sideQuests[0].isComplete && _tmpOptionalQuest1.fontStyle != FontStyles.Underline &&
                Quests.Instance.sideQuests[0].isFailed                                   == false)
            {
                _tmpOptionalQuest1.color = green;
                _tmpOptionalQuest1.DOColor(_originalTextColor, 1f).SetEase(Ease.InOutQuad);

                if (_imgOptionalQuestState1 != null) _imgOptionalQuestState1.sprite = _spriteFinishedState;

                _tmpOptionalQuest1.fontStyle = FontStyles.Underline;
            }
            else if (Quests.Instance.sideQuests[0].isFailed && Quests.Instance.sideQuests[0].isComplete == false &&
                     _tmpOptionalQuest1.fontStyle !=
                     FontStyles.Strikethrough)
            {
                _tmpOptionalQuest1.color = red;
                _tmpOptionalQuest1.DOColor(_originalTextColor, 1f).SetEase(Ease.InOutQuad);

                if (_imgOptionalQuestState1 != null) _imgOptionalQuestState1.sprite = _spriteFailedState;

                _tmpOptionalQuest1.fontStyle = FontStyles.Strikethrough;
            }
        }
        else
        {
            _tmpOptionalQuest1.text = string.Empty;

            if (_imgOptionalQuestState1 != null) _imgOptionalQuestState1.gameObject.SetActive(false);
        }

        if (Quests.Instance.sideQuests[1].isEnabled)
        {
            _tmpOptionalQuest2.text =
                $"{Quests.Instance.sideQuests[1].sideQuestText}" +
                $" ({Quests.Instance.sideQuests[1].goldReward}<sprite=0 index=4>)";

            if (Quests.Instance.sideQuests[1].isComplete && _tmpOptionalQuest2.fontStyle != FontStyles.Underline &&
                Quests.Instance.sideQuests[1].isFailed                                   == false)
            {
                _tmpOptionalQuest2.color = green;
                _tmpOptionalQuest2.DOColor(_originalTextColor, 1f).SetEase(Ease.InOutQuad);

                if (_imgOptionalQuestState2 != null) _imgOptionalQuestState2.sprite = _spriteFinishedState;

                _tmpOptionalQuest2.fontStyle = FontStyles.Underline;
            }
            else if (Quests.Instance.sideQuests[1].isFailed && Quests.Instance.sideQuests[1].isComplete == false &&
                     _tmpOptionalQuest2.fontStyle !=
                     FontStyles.Strikethrough)
            {
                _tmpOptionalQuest2.color = red;
                _tmpOptionalQuest2.DOColor(_originalTextColor, 1f).SetEase(Ease.InOutQuad);

                if (_imgOptionalQuestState2 != null) _imgOptionalQuestState2.sprite = _spriteFailedState;

                _tmpOptionalQuest2.fontStyle = FontStyles.Strikethrough;
            }
        }
        else
        {
            _tmpOptionalQuest2.text = string.Empty;

            if (_imgOptionalQuestState2 != null) _imgOptionalQuestState2.gameObject.SetActive(false);
        }

        if (Quests.Instance.sideQuests[2].isEnabled)
        {
            _tmpOptionalQuest3.text =
                $"{Quests.Instance.sideQuests[2].sideQuestText}" +
                $" ({Quests.Instance.sideQuests[2].goldReward}<sprite=0 index=4>)";

            if (Quests.Instance.sideQuests[2] != null                          &&
                Quests.Instance.sideQuests[2].isComplete                       &&
                _tmpOptionalQuest3.fontStyle           != FontStyles.Underline &&
                Quests.Instance.sideQuests[2].isFailed == false)
            {
                _tmpOptionalQuest3.color = green;
                _tmpOptionalQuest3.DOColor(_originalTextColor, 1f).SetEase(Ease.InOutQuad);

                if (_imgOptionalQuestState3 != null) _imgOptionalQuestState3.sprite = _spriteFinishedState;

                _tmpOptionalQuest3.fontStyle = FontStyles.Underline;
            }
            else if (Quests.Instance.sideQuests[2].isFailed && Quests.Instance.sideQuests[2].isComplete == false &&
                     _tmpOptionalQuest3.fontStyle !=
                     FontStyles.Strikethrough)
            {
                _tmpOptionalQuest3.color = red;
                _tmpOptionalQuest3.DOColor(_originalTextColor, 1f).SetEase(Ease.InOutQuad);

                if (_imgOptionalQuestState3 != null) _imgOptionalQuestState3.sprite = _spriteFailedState;

                _tmpOptionalQuest3.fontStyle = FontStyles.Strikethrough;
            }
        }
        else
        {
            _tmpOptionalQuest3.text = string.Empty;

            if (_imgOptionalQuestState3 != null) _imgOptionalQuestState3.gameObject.SetActive(false);
        }
    }
}