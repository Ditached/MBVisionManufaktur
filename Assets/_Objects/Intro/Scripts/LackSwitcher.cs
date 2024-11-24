using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class LackSwitcher : MonoBehaviour
{
    public float fadeOutSpeed = 1f;

    private static readonly int MainTransitionSlider = Shader.PropertyToID("_MainTransitionSlider");
    [Header("References")] public LackConfigCollection lackConfigCollection;
    public ChipState chipState;

    public MeshRenderer lackMeshRenderer;

    [Header("Debug")] [ReadOnly] public LackConfig activeLackConfig;

    private MaterialPropertyBlock _materialPropertyBlock;

    private void Awake()
    {
        //_chipStateReactor.OnChipStateChanged.AddListener(OnChipStateChanged);

        _materialPropertyBlock = new MaterialPropertyBlock();
        lackMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    private void HandleChipState()
    {
        if (!chipState.IsAnySensorActive() || chipState.AreMultipleSensorsActive())
        {
            EnsureFadeOut();
            materialChangeRequested = false;
            activeLackConfig = null;
            return;
        }

        var activeSensor = chipState.GetFirstActiveSensor();
        activeLackConfig = lackConfigCollection.lackConfigs[activeSensor];
        var activeMaterial = activeLackConfig.material;

        if (activeMaterial == null)
        {
            Debug.LogWarning($"LackSwitcher: LackConfig {activeLackConfig.name} has no material assigned.");
            return;
        }

        if (activeMaterial.name.Replace(" (Instance)", "") != lackMeshRenderer.material.name.Replace(" (Instance)", ""))
        {
            SwitchToMaterial(activeMaterial);
        }
        else
        {
            EnsureFadeIn();
        }
    }

    private void EnsureFadeIn()
    {
        fadingOutCurrentMat = false;
    }

    [ReadOnly] public bool fadingOutCurrentMat;
    [ReadOnly] public bool materialChangeRequested;
    private Material targetMaterial;
    

    private void Update()
    {
        HandleChipState();
        
        if (materialChangeRequested)
        {
            fadingOutCurrentMat = true;

            if (_materialPropertyBlock.GetFloat(MainTransitionSlider) <= 0f)
            {
                lackMeshRenderer.material = targetMaterial;
                materialChangeRequested = false;
                fadingOutCurrentMat = false;
            }
        }
        
        
        MoveLackSliderValueTowards(fadingOutCurrentMat ? 0f : 1f);
    }

    [ReadOnly] public float debugTargetValue;
    [ReadOnly] public float debugCurrentValue;
    [ReadOnly] public float propertyBlockValue;

    private void MoveLackSliderValueTowards(float targetValue)
    {
        debugTargetValue = targetValue;
        var val = _materialPropertyBlock.GetFloat(MainTransitionSlider);
        val = Mathf.MoveTowards(val, targetValue, Time.deltaTime * fadeOutSpeed);
        
        debugCurrentValue = Mathf.MoveTowards(debugCurrentValue, targetValue, Time.deltaTime * fadeOutSpeed);
        
        _materialPropertyBlock.SetFloat(MainTransitionSlider, val);
        propertyBlockValue = _materialPropertyBlock.GetFloat(MainTransitionSlider);
        lackMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }


    private void EnsureFadeOut()
    {
        fadingOutCurrentMat = true;
    }

    private Tween matTween;

    private void SwitchToMaterial(Material activeMaterial)
    {
        materialChangeRequested = true;
        targetMaterial = activeMaterial;
    }
}