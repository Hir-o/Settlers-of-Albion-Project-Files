using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventUiUpdater : MonoBehaviour
{
    private static EventUiUpdater _instance;
    public static  EventUiUpdater Instance => _instance;

    [SerializeField] private GameObject      _panelEvent;
    [SerializeField] private TextMeshProUGUI _tmpDesc, _tmpBenefit;

    public bool isEventEnabled, showEvent;

    private int _counter = 0, _gold;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowEventPanel()
    {
        showEvent = false;
        
        switch (_counter)
        {
            case 0:
                _panelEvent.SetActive(true);
                _gold = 50;

                ResourcesController.Instance.gold += _gold;
                ResourcesController.Instance.UpdateOnlyResourceUIText();
                _tmpDesc.text =
                    $"My Liege, our colonies have sent us an aid of {_gold}<sprite=0 index=4> to defend against the enemy invasion and to ensure further development of our settlements. Use them well.";

                _tmpBenefit.text = $"\"You have just received {_gold}<sprite=0 index=4>\"";
                break;
            case 1:
                _panelEvent.SetActive(true);
                _gold = 100;

                ResourcesController.Instance.gold += _gold;
                ResourcesController.Instance.UpdateOnlyResourceUIText();

                _tmpDesc.text =
                    $"My Liege, our colonies have sent us an aid of {_gold}<sprite=0 index=4> to defend against the enemy invasion and to ensure further development of our settlements. Use them well.";

                _tmpBenefit.text = $"\"You have just received {_gold}<sprite=0 index=4>\"";
                break;
            case 2:
                _panelEvent.SetActive(true);
                _gold = 100;

                ResourcesController.Instance.gold += _gold;
                ResourcesController.Instance.UpdateOnlyResourceUIText();

                _tmpDesc.text =
                    $"My Liege, our colonies have sent us an aid of {_gold}<sprite=0 index=4> to defend against the enemy invasion and to ensure further development of our settlements. Use them well.";
                break;
            case 3:
                _panelEvent.SetActive(true);
                _gold = 120;

                ResourcesController.Instance.gold += _gold;
                ResourcesController.Instance.UpdateOnlyResourceUIText();

                _tmpDesc.text =
                    $"My Liege, our colonies have sent us an aid of {_gold}<sprite=0 index=4> to defend against the enemy invasion and to ensure further development of our settlements. Use them well.";

                _tmpBenefit.text = $"\"You have just received {_gold}<sprite=0 index=4>\"";
                break;
            case 4:
                _panelEvent.SetActive(true);
                _gold = 200;

                ResourcesController.Instance.gold += _gold;
                ResourcesController.Instance.UpdateOnlyResourceUIText();

                _tmpDesc.text =
                    $"My Liege, our colonies have sent us an aid of {_gold}<sprite=0 index=4>. This is the final aid we are getting from them. Everything is in our hands from now on.";

                isEventEnabled = false;

                _tmpBenefit.text = $"\"You have just received {_gold}<sprite=0 index=4>\"";
                break;
        }

        _counter++;
    }
}