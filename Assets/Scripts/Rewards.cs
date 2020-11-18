using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards : MonoBehaviour
{
    public void AddReward(int goldReward)
    {
        ResourcesController.Instance.gold += goldReward;
        
        ResourcesController.Instance.UpdateOnlyResourceUIText();
        
        ObjectHolder.Instance.UpdateAllButtonStates();
    }
}
