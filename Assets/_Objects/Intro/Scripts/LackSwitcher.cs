using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class LackSwitcher : MonoBehaviour
{
    [Header("References")]
    public LackConfigCollection lackConfigCollection;
    public ChipStateReactor _chipStateReactor;
    
    [Header("Debug")]
    [ReadOnly] public LackConfig activeLackConfig;

    private void Awake()
    {
        _chipStateReactor.OnChipStateChanged.AddListener(OnChipStateChanged);
    }

    private void OnChipStateChanged(int index)
    {
        if (index == -1)
        {
            activeLackConfig = null;
            return;
        }

        if (index >= lackConfigCollection.lackConfigs.Length)
        {
            activeLackConfig = null;
            return;
        }
            
        activeLackConfig = lackConfigCollection.lackConfigs[index];
    }
}
