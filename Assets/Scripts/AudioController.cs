using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private static AudioController _instance;
    public static  AudioController Instance => _instance;

    [HideInInspector] public AudioSource audioSource;

    private bool toggleAudio;

    [BoxGroup("Sfx UI")]
    [SerializeField]
    private AudioClip _sfxButtonClick, _sfxTileLevelSelect, _sfxLevelChoose, _sfxMarketBuy, _sfxMarketSell;

    [BoxGroup("Sfx Game State")]
    [SerializeField] private AudioClip _sfxGameWon, _sfxGameLose, _sfxSideQuestComplete, _sfxSideQuestFail;

    [BoxGroup("Sfx Units")]
    [SerializeField] private AudioClip _sfxUnitSelect,
                                       _sfxUnitRecruit,
                                       _sfxFormSettlement,
                                       _sfxToggleQuests,
                                       _sfxBuildingUpgrade,
                                       _sfxUnitUpgrade,
                                       _sfxBuildWorkshop,
                                       _sfxArcherAttack,
                                       _sfxWeaponImpact,
                                       _sfxEndTurn;

    [BoxGroup("Sfx Units")]
    [SerializeField] private AudioClip[] _sfxFriendlyMeleAttacks, _sfxEnemyAttacks;

    [BoxGroup("Sfx Units")]
    [SerializeField] private AudioClip _sfxHammerAttack;

    [BoxGroup("Steps")]
    [SerializeField] private AudioClip[] _sfxSteps;

    public int   maxUISoundsPlayed  = 3, maxUnitSelectedSoundsPlayed = 1, maxMeleeAttackSoundsPlayed = 1;
    public float soundCapResetSpeed = 0.55f;

    private float _timePassed;
    private int   _uiSoundsPlayed, _unitSelectedSoundsPlayed, _meleeAttackSoundsPlayed;

    private bool _isGameEndSoundPlayed;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        audioSource = Camera.main.GetComponent<AudioSource>();
        toggleAudio = ES3.Load(SaveController.Instance.keyAudio, true);

        if (toggleAudio == false && AudioListener.volume != 0f) AudioListener.volume = 0f;
    }

    private void Update()
    {
        _timePassed += Time.deltaTime;

        if (_timePassed >= soundCapResetSpeed)
        {
            _uiSoundsPlayed           = 0;
            _unitSelectedSoundsPlayed = 0;
            _timePassed               = 0;
            _meleeAttackSoundsPlayed  = 0;
        }
    }

    public void ToggleSound()
    {
        toggleAudio = !toggleAudio;

        if (toggleAudio)
        {
            AudioListener.volume                   = 1f;
            AudioUI.Instance.imgAudioButton.sprite = AudioUI.Instance.sprAudioOn;

            SFXButtonClick();
        }
        else
        {
            AudioListener.volume                   = 0f;
            AudioUI.Instance.imgAudioButton.sprite = AudioUI.Instance.sprAudioOff;
        }

        SaveController.Instance.SaveAudio();
    }

    public void SFXButtonClick()
    {
        _uiSoundsPlayed++;

        if (_uiSoundsPlayed > maxUISoundsPlayed) return;

        audioSource.PlayOneShot(_sfxButtonClick);
        //EazySoundManager.PlayUISound(_sfxButtonClick);
    }

    public void SFXTileLevelSelect()
    {
        _uiSoundsPlayed++;

        if (_uiSoundsPlayed > maxUISoundsPlayed) return;

        audioSource.PlayOneShot(_sfxTileLevelSelect);
        //EazySoundManager.PlayUISound(_sfxTileLevelSelect);
    }

    public void SFXTileLevelChoose()
    {
        _uiSoundsPlayed++;

        if (_uiSoundsPlayed > maxUISoundsPlayed) return;

        audioSource.PlayOneShot(_sfxLevelChoose);
//        EazySoundManager.PlayUISound(_sfxLevelChoose);
    }

    public void SFXMarketBuy()
    {
        audioSource.PlayOneShot(_sfxMarketBuy);
        //EazySoundManager.PlaySound(_sfxMarketBuy);
    }

    public void SFXMarketSell()
    {
        audioSource.PlayOneShot(_sfxMarketSell);
        //EazySoundManager.PlaySound(_sfxMarketSell);
    }

    public void SFXLevelWin()
    {
        if (_isGameEndSoundPlayed) return;

        _isGameEndSoundPlayed = true;

        if (audioSource != null) audioSource.PlayOneShot(_sfxGameWon);
        //EazySoundManager.PlaySound(_sfxGameWon);
    }

    public void SFXLevelLose()
    {
        if (_isGameEndSoundPlayed) return;

        _isGameEndSoundPlayed = true;

        if (audioSource != null) audioSource.PlayOneShot(_sfxGameLose);
        //EazySoundManager.PlaySound(_sfxGameLose);
    }

    public void SFXSideQuestCompleted()
    {
        if (audioSource != null) audioSource.PlayOneShot(_sfxSideQuestComplete);
//        EazySoundManager.PlaySound(_sfxSideQuestComplete);
    }

    public void SFXSideQuestFailed()
    {
        if (audioSource != null) audioSource.PlayOneShot(_sfxSideQuestFail);
//        EazySoundManager.PlaySound(_sfxSideQuestFail);
    }

    public void SFXRecruitUnit()
    {
        _unitSelectedSoundsPlayed++;

        if (_unitSelectedSoundsPlayed > maxUnitSelectedSoundsPlayed) return;

        audioSource.PlayOneShot(_sfxUnitRecruit, .8f);
//        EazySoundManager.PlaySound(_sfxUnitRecruit, .8f);
    }

    public void SFXToggleQuests()
    {
        audioSource.PlayOneShot(_sfxToggleQuests, .8f);
//        EazySoundManager.PlaySound(_sfxToggleQuests, .8f);
    }

    public void SFXFormSettlementOrOutpost()
    {
        audioSource.PlayOneShot(_sfxFormSettlement);
//        EazySoundManager.PlaySound(_sfxFormSettlement);
    }

    public void SFXUpgradeBuilding()
    {
        audioSource.PlayOneShot(_sfxBuildingUpgrade);
//        EazySoundManager.PlaySound(_sfxBuildingUpgrade);
    }

    public void SFXUpgradeUnit()
    {
        audioSource.PlayOneShot(_sfxUnitUpgrade);
//        EazySoundManager.PlaySound(_sfxUnitUpgrade);
    }

    public void SFXBuildWorkshop()
    {
        audioSource.PlayOneShot(_sfxBuildWorkshop);
//        EazySoundManager.PlaySound(_sfxBuildWorkshop);
    }

    public void SFXEndTurn()
    {
        audioSource.PlayOneShot(_sfxEndTurn, .8f);
//        EazySoundManager.PlaySound(_sfxEndTurn, .8f);
    }

    public void SFXSelectUnit()
    {
        _unitSelectedSoundsPlayed++;

        if (_unitSelectedSoundsPlayed > maxUnitSelectedSoundsPlayed) return;

        audioSource.PlayOneShot(_sfxUnitSelect, .8f);
//        EazySoundManager.PlaySound(_sfxUnitSelect, .8f);
    }

    public void SFXShootArrow()
    {
        audioSource.PlayOneShot(_sfxArcherAttack);
//        EazySoundManager.PlaySound(_sfxArcherAttack);
    }

    public void SFXWeaponImpact()
    {
        audioSource.PlayOneShot(_sfxWeaponImpact);
//        EazySoundManager.PlaySound(_sfxWeaponImpact);
    }

    public void SFXMeleeAttack()
    {
        _meleeAttackSoundsPlayed++;

        if (_meleeAttackSoundsPlayed > maxMeleeAttackSoundsPlayed) return;
        
        if (ObjectHolder.Instance.cellGrid.CurrentPlayerNumber == 0)
        {
            audioSource.PlayOneShot(_sfxFriendlyMeleAttacks[Random.Range(0, _sfxFriendlyMeleAttacks.Length)]);
//            EazySoundManager.PlaySound(_sfxFriendlyMeleAttacks[Random.Range(0, _sfxFriendlyMeleAttacks.Length)]);
        }
        else
        {
            audioSource.PlayOneShot(_sfxEnemyAttacks[Random.Range(0, _sfxEnemyAttacks.Length)]);
//            EazySoundManager.PlaySound(_sfxEnemyAttacks[Random.Range(0, _sfxEnemyAttacks.Length)]);
        }
    }

    public void SFXHammerAttack()
    {
        audioSource.PlayOneShot(_sfxHammerAttack);
//        EazySoundManager.PlaySound(_sfxHammerAttack);
    }

    public void SFXSteps()
    {
        StopAllCoroutines();

        StartCoroutine(PlaySFXSteps());
    }

    private IEnumerator PlaySFXSteps()
    {
        for (int i = 0; i < 3; i++)
        {
            int random = Mathf.RoundToInt(Random.Range(0, _sfxSteps.Length));

            audioSource.PlayOneShot(_sfxSteps[random], .8f);

            yield return new WaitForSeconds(Random.Range(.1f, .3f));
        }
    }
}