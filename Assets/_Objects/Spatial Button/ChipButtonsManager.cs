using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ChipButtonsManager : MonoBehaviour
{
    public List<ChipStateButton> chipStateButtons;

    
    [Button]
    private void OnValidate()
    {
        chipStateButtons = new List<ChipStateButton>(GetComponentsInChildren<ChipStateButton>());
        
        for (int i = 0; i < chipStateButtons.Count; i++)
        {
            chipStateButtons[i].index = i;
            chipStateButtons[i].OnValidate();
        }
    }
}
