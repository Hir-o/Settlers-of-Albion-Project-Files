using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceAndWavePanels : MonoBehaviour
{
    public GameObject resourcePanel, wavePanel, market, turnIcon;

    private void Start() { UpdateTopPanels(); }

    public void UpdateTopPanels()
    {
        switch (Quests.Instance.mainQuestType)
        {
            case Quests.MainQuestType.Battle:
                resourcePanel.SetActive(false);
                wavePanel.SetActive(false);
                market.SetActive(false);
                turnIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -4f);
                break;
            case Quests.MainQuestType.Defend:
                wavePanel.SetActive(true);
                break;
        }
    }
}