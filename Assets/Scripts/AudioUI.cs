using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioUI : MonoBehaviour
{
    private static AudioUI _instance;
    public static  AudioUI Instance => _instance;

    public Image imgAudioButton;

    public Sprite sprAudioOn, sprAudioOff;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        if (ES3.KeyExists(SaveController.Instance.keyAudio))
        {
            bool isAudioOn = ES3.Load(SaveController.Instance.keyAudio, true);

            if (isAudioOn)
                imgAudioButton.sprite = sprAudioOn;
            else
                imgAudioButton.sprite = sprAudioOff;
        }
    }

    public void ToggleSound()
    {
        AudioController.Instance.ToggleSound();
    }
}