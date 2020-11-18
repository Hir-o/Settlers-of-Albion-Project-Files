using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public Image imgBlank;

    public RectTransform rectImgTile, rectImgNumber;

    public int levelIndex = 1;
    public bool isUnlocked;

    [HideInInspector]
    public RectTransform rectTransform, rectImgBlank;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectImgBlank = imgBlank.GetComponent<RectTransform>();

        rectImgNumber.DOSizeDelta(new Vector2(0f, 0f), 0f);

        if (ES3.KeyExists(SaveController.Instance.keyLevelIndex) == false)
        {
            SaveController.Instance.SaveLevelIndex(1);
            return;
        }

        int maxLevelUnlocked = ES3.Load(SaveController.Instance.keyLevelIndex, 1);

        if (levelIndex <= maxLevelUnlocked)
            isUnlocked = true;
        else
            isUnlocked = false;
    }
}