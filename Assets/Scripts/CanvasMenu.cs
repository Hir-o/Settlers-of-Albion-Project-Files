using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lean.Gui;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _rectDiscord;

    [SerializeField] private float _discordImageRotateDuration = .5f, _fadeDuration = 1f;

    [SerializeField] private Ease _discordEase          = Ease.OutBounce,
                                  _fadeEase             = Ease.InCubic,
                                  _hoverLevelButtonEase = Ease.OutSine;

    [SerializeField] private TextMeshProUGUI _version;

    [SerializeField] private string _versionText;

    [SerializeField] private CanvasGroup _cgDiscord, _cgFabletale, _cgLevels, _cgWaves;

    [SerializeField] private LevelSelectButton[] _btnLevelSelect;

    [SerializeField] private TextMeshProUGUI _tmpSelectLevel;

    [SerializeField] private Image _selectLevelLine, _imgAudioButton;

    [SerializeField] private Sprite _sprAudioOn, _sprAudioOff;

    [SerializeField] private Image _imgLogoSettlersOf, _imgLogoAlbion, _imgLogoCity;

    [SerializeField] private RectTransform _rectLogo;

    [SerializeField] private Button _btnToggleAudio;

    private int _currentSortOrder, _versionNumberClicked;

    public bool toggleAudio, isLevelStarted, _areCheatsEnabled;

    private void Start()
    {
        _version.text = _versionText;

        _cgDiscord.alpha   = 0f;
        _cgFabletale.alpha = 0f;
        _cgLevels.alpha    = 1f;
        _cgWaves.alpha     = 0f;

        _tmpSelectLevel.DOFade(0f, 0f);
        _selectLevelLine.DOFade(0f, 0f);
        _imgAudioButton.DOFade(0f, 0f);
        _imgLogoSettlersOf.DOFade(0f, 0f);
        _imgLogoAlbion.DOFade(0f, 0f);
        _imgLogoCity.DOFade(0f, 0f);

        StartCoroutine(InitFadeIn());

        if (ES3.KeyExists(SaveController.Instance.keyAudio))
        {
            bool isAudioOn = ES3.Load(SaveController.Instance.keyAudio, true);

            if (isAudioOn)
            {
                _imgAudioButton.sprite = _sprAudioOn;
                AudioListener.volume   = 1f;
            }
            else
            {
                _imgAudioButton.sprite = _sprAudioOff;
                AudioListener.volume   = 0f;
            }
        }
    }

    private IEnumerator InitFadeIn()
    {
        Sequence fadeSequence = DOTween.Sequence();

        fadeSequence.Append(_cgLevels.DOFade(0f, _fadeDuration).SetDelay(1f).SetEase(_fadeEase)
                                     .OnComplete(UpdateButtons));
        fadeSequence.Append(_cgFabletale.DOFade(1f, _fadeDuration).SetDelay(1f).SetDelay(2f).SetEase(_fadeEase));

        _imgLogoSettlersOf.DOFade(1f, _fadeDuration).SetEase(_fadeEase);
        _imgLogoAlbion.DOFade(1f, _fadeDuration).SetEase(_fadeEase).SetDelay(1f);
        _imgLogoCity.DOFade(1f, _fadeDuration).SetEase(_fadeEase).SetDelay(1.5f);

        _rectLogo.DOLocalMoveY(216f, 2f).SetEase(Ease.OutSine);

        _imgAudioButton.DOFade(1f, 2f).SetEase(_fadeEase).SetDelay(3f);

        _cgDiscord.DOFade(1f, _fadeDuration).SetEase(_fadeEase).SetDelay(3f);

        _tmpSelectLevel.DOFade(1f, 2f).SetEase(_fadeEase).SetDelay(3f);
        _tmpSelectLevel.GetComponent<RectTransform>().DOLocalMoveX(400f, 2f).SetEase(Ease.OutCubic).SetDelay(3f);

        _selectLevelLine.DOFade(1f, 2f).SetEase(_fadeEase).SetDelay(3f);
        _selectLevelLine.gameObject.GetComponent<RectTransform>().DOLocalMoveX(330f, 2f).SetEase(Ease.OutCubic)
                        .SetDelay(3f);

        yield return null;
    }

    public void UpdateButtons()
    {
        foreach (LevelSelectButton button in _btnLevelSelect)
        {
            if (button.isUnlocked == false) continue;

            float delay    = Random.Range(.4f, .8f);
            float duration = Random.Range(.5f, 1f);

            button.rectImgBlank
                  .DOLocalRotate(new Vector3(0f, 360f, 0f), duration)
                  .SetEase(_hoverLevelButtonEase).SetDelay(delay).OnComplete(() => DisableBlankImg(button, duration));
        }

        _cgWaves.DOFade(1f, 2f).SetEase(_fadeEase).SetDelay(1f);
    }

    private void DisableBlankImg(LevelSelectButton button, float duration)
    {
        //float duration = Random.Range(.2f, 1f);

        button.imgBlank.gameObject.SetActive(false);

        button.rectImgTile
              .DOLocalRotate(new Vector3(0f, 360f, 0f), duration)
              .SetEase(_hoverLevelButtonEase);

        button.rectImgNumber.DOSizeDelta(new Vector2(50f, 50f), duration).SetEase(Ease.OutSine).SetDelay(duration);
    }

    public void ToggleSound()
    {
        toggleAudio = !toggleAudio;

        if (toggleAudio)
        {
            AudioListener.volume   = 1f;
            _imgAudioButton.sprite = _sprAudioOn;

            AudioController.Instance.SFXButtonClick();
        }
        else
        {
            AudioListener.volume   = 0f;
            _imgAudioButton.sprite = _sprAudioOff;
        }

        SaveController.Instance.SaveAudio();
    }

    public void DiscordMouseEnter()
    {
        //DOTween.KillAll();
        _rectDiscord.DOLocalRotate(new Vector3(0f, 0f, -12f), _discordImageRotateDuration).SetEase(_discordEase);
    }

    public void DiscordMouseExit()
    {
        //DOTween.KillAll();
        _rectDiscord.DOLocalRotate(new Vector3(0f, 0f, 0f), _discordImageRotateDuration).SetEase(_discordEase);
    }

    public void LevelButtonMouseEnter(RectTransform rect)
    {
        if (isLevelStarted) return;

        if (rect.GetComponent<LevelSelectButton>().imgBlank.gameObject.activeSelf) return;

        //DOTween.KillAll();
        rect.DOScale(new Vector3(2.3f, 2.3f, 1f), .3f).SetEase(_hoverLevelButtonEase);

        rect.GetComponent<Canvas>().sortingOrder                                                 = 100;
        rect.GetComponent<LevelSelectButton>().rectImgNumber.GetComponent<Canvas>().sortingOrder = 101;
        
        AudioController.Instance.SFXTileLevelSelect();
    }

    public void LevelButtonMouseExit(RectTransform rect)
    {
        if (isLevelStarted) return;

        if (rect.GetComponent<LevelSelectButton>().imgBlank.gameObject.activeSelf) return;

        //DOTween.KillAll();
        rect.DOScale(new Vector3(2f, 2f, 1f), .3f).SetEase(_hoverLevelButtonEase);

        rect.GetComponent<Canvas>().sortingOrder                                                 = 0;
        rect.GetComponent<LevelSelectButton>().rectImgNumber.GetComponent<Canvas>().sortingOrder = 10;
    }

    public void StartLevel(LevelSelectButton button)
    {
        DOTween.KillAll();

        button.GetComponent<Canvas>().sortingOrder               = 200;
        button.rectImgNumber.GetComponent<Canvas>().sortingOrder = 210;

        _cgLevels.interactable = false;
        _cgLevels.DOFade(1f, 1f).SetEase(_fadeEase);

        isLevelStarted = true;

        _cgDiscord.interactable      = false;
        _btnToggleAudio.interactable = false;

        foreach (var levelButton in _btnLevelSelect)
        {
            levelButton.GetComponent<CanvasGroup>().interactable = false;
            levelButton.GetComponent<LeanButton>().enabled       = false;
        }

        button.GetComponent<RectTransform>().DOLocalMove(Vector3.zero, 1f)
              .SetEase(Ease.InOutSine)
              .SetDelay(1f);

        button.GetComponent<RectTransform>().DOScale(new Vector3(2.8f, 2.8f), 1f)
              .SetEase(Ease.InOutSine)
              .SetDelay(1.5f).OnComplete(() => LevelController.Instance.StartLevelAtIndex(button.levelIndex + 1));

        _cgDiscord.DOFade(0f, 1f).SetEase(_fadeEase);
        _cgFabletale.DOFade(0f, 1f).SetEase(_fadeEase);
        _cgWaves.DOFade(0f, 1f).SetEase(_fadeEase);

        _tmpSelectLevel.DOFade(0f, 1f).SetEase(_fadeEase);
        _selectLevelLine.DOFade(0f, 1f).SetEase(_fadeEase);
        _imgAudioButton.DOFade(0f, 1f).SetEase(_fadeEase);
        _imgLogoSettlersOf.DOFade(0f, 1f).SetEase(_fadeEase);
        _imgLogoAlbion.DOFade(0f, 1f).SetEase(_fadeEase);
        _imgLogoCity.DOFade(0f, 1f).SetEase(_fadeEase);
        
        AudioController.Instance.SFXTileLevelChoose();
    }

    public void UlockAllLevels()
    {
        if (_areCheatsEnabled == false) return;
        
        _versionNumberClicked++;

        if (_versionNumberClicked >= 5)
        {
            foreach (LevelSelectButton button in _btnLevelSelect) { button.isUnlocked = true; }
            
            UpdateButtons();
        }
    }

    public void OpenDiscordLink() { Application.ExternalEval("window.open(\"https://discord.gg/UQMAMb8\")"); }

    public void ArmorGamesPageLink() { Application.ExternalEval("window.open(\"https://armor.ag/MoreGames\")"); }

    public void ArmorGamesFacebookPageLink()
    {
        Application.ExternalEval("window.open(\"https://www.facebook.com/ArmorGames\")");
    }
}