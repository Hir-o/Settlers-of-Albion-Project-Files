using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickToggleQuestsButton : MonoBehaviour
{
    private Button _btnToggleQuests;

    [SerializeField] private GameObject QuestsGameObject;

    private void Awake()
    {
        _btnToggleQuests = GetComponent<Button>();
        _btnToggleQuests.onClick.AddListener(ToggleQuests);
    }

    public void ToggleQuests()
    {
        QuestsGameObject.SetActive(!QuestsGameObject.activeSelf);
        
        AudioController.Instance.SFXToggleQuests();
    }
}
