using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsUI : MonoBehaviour
{
    [SerializeField] private GameObject _panelOptions;

    public void ToggleOptions() { _panelOptions.SetActive(!_panelOptions.activeSelf); }
}