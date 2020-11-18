using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasButtonSfx : MonoBehaviour
{
    public void PlayClickSfx()
    {
        AudioController.Instance.SFXButtonClick();
    }
}
