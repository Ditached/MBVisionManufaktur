using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;



public class LackSwitcher : MonoBehaviour
{
    [FormerlySerializedAs("fadeOutSpeed")] public float fadeSpeed = 1f;

    private static readonly int MainTransitionSlider = Shader.PropertyToID("_MainTransitionSlider");
    [Header("References")]
    public LackConfigCollection lackConfigCollection;
    public ChipState chipState;
    public MeshRenderer lackMeshRenderer;

    public Transform dissolverPlane;
    public float dissolverPlaneFadeOutY = 0f;
    public float dissolverPlaneFadeInY = 5f;
    
    public Transform sandstoneWorld;
    public Transform crystalWorld;
    public Transform jungleWorld;

    private Transform currentWorld;
    
    public Light undergroundLight;
    public ParticleSystem particles;


    [Header("Debug")] [ReadOnly] public LackConfig activeLackConfig;

    private MaterialPropertyBlock _materialPropertyBlock;

    private void Awake()
    {
        jungleWorld.gameObject.SetActive(false);
        crystalWorld.gameObject.SetActive(false);
        sandstoneWorld.gameObject.SetActive(false);

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
                if(currentWorld != null) currentWorld.gameObject.SetActive(false);

                switch (activeLackConfig.lackWorld)
                {
                    case LackWorld.Sandstone:
                        currentWorld = sandstoneWorld;
                        break;
                    case LackWorld.Crystal:
                        currentWorld = crystalWorld;
                        break;
                    case LackWorld.Jungle:
                        currentWorld = jungleWorld;
                        break;
                }
                
                currentWorld.gameObject.SetActive(true);
                lackMeshRenderer.material = targetMaterial;
                materialChangeRequested = false;
                fadingOutCurrentMat = false;
            }
        }
        
        
        FadeTo(fadingOutCurrentMat ? 0f : 1f);
    }

    [ReadOnly] public float debugTargetValue;
    [ReadOnly] public float fadeValue;
    [ReadOnly] public float propertyBlockValue;

    private void FadeTo(float targetValue)
    {
        var color = activeLackConfig == null ? Color.cyan : activeLackConfig.mainColor;
        undergroundLight.color = Color.Lerp(undergroundLight.color, color, Time.deltaTime * fadeSpeed);
        
        var main = particles.main;
        var startColor = main.startColor;
        ParticleSystem.MinMaxGradient newGradient = new ParticleSystem.MinMaxGradient( Color.Lerp(startColor.color, color, Time.deltaTime * fadeSpeed) );
        main.startColor = newGradient;
        
           debugTargetValue = targetValue;
        var val = _materialPropertyBlock.GetFloat(MainTransitionSlider);
        val = Mathf.MoveTowards(val, targetValue, Time.deltaTime * fadeSpeed);
        
        fadeValue = Mathf.MoveTowards(fadeValue, targetValue, Time.deltaTime * fadeSpeed);
        
        var planePosY = Mathf.Lerp(dissolverPlaneFadeOutY, dissolverPlaneFadeInY, val);
        dissolverPlane.localPosition = new Vector3(dissolverPlane.localPosition.x, planePosY, dissolverPlane.localPosition.z);

        
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