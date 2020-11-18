using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    private static SaveController _instance;
    public static  SaveController Instance => _instance;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public string keyAudio      = "Audio",
                  keyLevelIndex = "Level";

    public void SaveAudio()
    {
        bool isAudioOn = AudioListener.volume > 0;

        ES3.Save(keyAudio, isAudioOn);
    }

    public void SaveLevelIndex(int levelIndex)
    {
        int currentLevelIndex = ES3.Load(keyLevelIndex, 1);
        
        if (currentLevelIndex < levelIndex)
            ES3.Save(keyLevelIndex, levelIndex);
    }
}