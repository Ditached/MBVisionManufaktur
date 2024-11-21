using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class LackSwitcher : MonoBehaviour
{
    [Header("References")]
    public LackConfigCollection lackConfigCollection;
    public ChipStateReactor _chipStateReactor;
    
    public MeshRenderer lackMeshRenderer;
    public int[] lackMaterialIndexes;
    
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
        
        var activeMaterial = activeLackConfig.material;
        
        if (activeMaterial == null)
        {
            Debug.LogWarning($"LackSwitcher: LackConfig {activeLackConfig.name} has no material assigned.");
            return;
        }
        
        var materials = lackMeshRenderer.materials; // Get copy of array
        for (var i = 0; i < materials.Length; i++)
        {
            if(Array.Exists(lackMaterialIndexes, element => element == i))
            {
                materials[i] = activeMaterial;
            }
        }
        lackMeshRenderer.materials = materials; // Set entire array back
    }
}
