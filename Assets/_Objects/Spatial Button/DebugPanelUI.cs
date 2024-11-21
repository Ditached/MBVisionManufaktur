using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DebugPanelUI : MonoBehaviour
{
    public TMP_Text appStateText;
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

    private void Update()
    {
        appStateText.text = UpdatePackage.globalAppState.ToString();
    }
}
