using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIUpdater : MonoBehaviour
{
    private static TutorialUIUpdater _instance;
    public static  TutorialUIUpdater Instance => _instance;

    [SerializeField] private GameObject _tutorialPanelMenu;

    [SerializeField] private GameObject[] _tutorialPanels;

    [SerializeField] private GameObject tut_defend_4, tut_battle_2_7;

    [SerializeField] private GameObject expansionOnTheLowlandsPanel;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public void ToggleTutorialPanel() { _tutorialPanelMenu.SetActive(!_tutorialPanelMenu.activeSelf); }

    public void OpenTutorial(int index)
    {
        foreach (GameObject panel in _tutorialPanels) panel.SetActive(false); 
        
        if (_tutorialPanels[index].activeSelf == false)
            _tutorialPanels[index].SetActive(true);
    }

    public void OpenPanelTutDefend4()
    {
        if (tut_defend_4 != null)
            tut_defend_4.SetActive(true);
    }

    public void OpenPanelTutBattle27()
    {
        if (tut_battle_2_7 != null)
            tut_battle_2_7.SetActive(true);
    }

    public void OpenPanelExpansionOnTheLowlands()
    {
        if(expansionOnTheLowlandsPanel != null)
            expansionOnTheLowlandsPanel.SetActive(true);
    }

    public void ExpansionOnTheLowlands(bool value)
    {
        if (DifficultyController.Instance != null) { DifficultyController.Instance.DisableSlimesAndWolves(value); }
    }
}