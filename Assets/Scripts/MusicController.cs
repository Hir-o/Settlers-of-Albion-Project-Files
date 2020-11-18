using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
    private static MusicController _instance;
    public static  MusicController Instance => _instance;

    public float musicVolume = .5f;
    public Ease  fadeEase    = Ease.OutSine;

    [ReorderableList]
    public List<AudioClip> _clips = new List<AudioClip>();

    private AudioSource _audioSource;
    private IEnumerator _musicCoroutine;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else
            Destroy(gameObject);

        _audioSource = GetComponent<AudioSource>();

        _audioSource.volume = 0f;

        _musicCoroutine = PlayMusic();
    }

    private void Start() { SceneManager.sceneLoaded += OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().buildIndex <= 1)
        {
            StopAllCoroutines();
            CancelInvoke();
            StopMusic();
        }
        else
        {
            if (_audioSource.isPlaying == false)
                StartCoroutine(PlayMusic());
        }
    }

    private IEnumerator PlayMusic()
    {
        foreach (AudioClip clip in _clips)
        {
            _audioSource.volume = 0f;
            
            _audioSource.clip = clip;
            _audioSource.Play();

            _audioSource.DOFade(musicVolume, 1f).SetEase(fadeEase);
            
            Invoke(nameof(FadeOutAudio), _audioSource.clip.length - 1f);

            yield return new WaitForSeconds(_audioSource.clip.length);
        }

        RestartMusic();
    }

    private void FadeOutAudio() { _audioSource.DOFade(0f, 1f).SetEase(fadeEase); }
    
    public void RestartMusic()
    {
        StopAllCoroutines();
        StartCoroutine(PlayMusic());
    }

    public void StopMusic()
    {
        StopAllCoroutines();

        _audioSource.DOFade(0f, 1f).SetEase(fadeEase).OnComplete(ExecuteStop);
    }

    private void ExecuteStop() { _audioSource.Stop(); }
}