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

    public ScalePlane sandstoneShadows;
    public ScalePlane crystalShadows;
    public ScalePlane jungleShadows;
    
    private ScalePlane currentShadows;
    
    public AudioSource sandstoneAudio;
    public AudioSource crystalAudio;
    public AudioSource jungleAudio;
    
    private AudioSource currentAudio;

    private Transform currentWorld;
    
    public Light undergroundLight;
    public ParticleSystem particles;

    [Header("World Colors")]
    public Color defaultColor = Color.white;
    public Color sandstoneColor = Color.red;
    public Color crystalColor = Color.blue;
    public Color jungleColor = Color.green;

    [Header("Debug")] [ReadOnly] public LackConfig activeLackConfig;

    private MaterialPropertyBlock _materialPropertyBlock;
    public event Action<LackWorld> OnWorldChanged;
    public event Action NoWorldActive;

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
            NoWorldActive?.Invoke();
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
                if(currentShadows != null) currentShadows.SetScalePercentage(0f);
                if (currentAudio != null) currentAudio.volume = 0f;

                switch (activeLackConfig.lackWorld)
                {
                    case LackWorld.Sandstone:
                        currentWorld = sandstoneWorld;
                        currentShadows = sandstoneShadows;
                        currentAudio = sandstoneAudio;
                        soundFXController.Play_Lack_RevealRed();
                        break;
                    case LackWorld.Crystal:
                        currentWorld = crystalWorld;
                        currentShadows = crystalShadows;
                        currentAudio = crystalAudio;
                        soundFXController.Play_Lack_RevealBlue();
                        break;
                    case LackWorld.Jungle:
                        currentWorld = jungleWorld;
                        currentShadows = jungleShadows;
                        currentAudio = jungleAudio;
                        soundFXController.Play_Lack_RevealGreen();
                        break;
                }
                
                OnWorldChanged?.Invoke(activeLackConfig.lackWorld);
                Debug.Log($"Switching to {activeLackConfig.lackWorld}");
                
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
        // Set light and particle colors based on the active world
        Color worldColor = defaultColor; // Default to white

        if (currentWorld == sandstoneWorld)
        {
            worldColor = sandstoneColor;
        }
        else if (currentWorld == crystalWorld)
        {
            worldColor = crystalColor;
        }
        else if (currentWorld == jungleWorld)
        {
            worldColor = jungleColor;
        }

        // Apply the color to underground light and particles
        undergroundLight.color = Color.Lerp(undergroundLight.color, worldColor, Time.deltaTime * fadeSpeed);
        
        var main = particles.main;
        var startColor = main.startColor;
        ParticleSystem.MinMaxGradient newGradient = new ParticleSystem.MinMaxGradient(Color.Lerp(startColor.color, worldColor, Time.deltaTime * fadeSpeed));
        main.startColor = newGradient;
        
        debugTargetValue = targetValue;
        var val = _materialPropertyBlock.GetFloat(MainTransitionSlider);
        val = Mathf.MoveTowards(val, targetValue, Time.deltaTime * fadeSpeed);
        
        fadeValue = Mathf.MoveTowards(fadeValue, targetValue, Time.deltaTime * fadeSpeed);
        
        var planePosY = Mathf.Lerp(dissolverPlaneFadeOutY, dissolverPlaneFadeInY, val);
        dissolverPlane.localPosition = new Vector3(dissolverPlane.localPosition.x, planePosY, dissolverPlane.localPosition.z);

        if(currentShadows != null) currentShadows.SetScalePercentage(val);
        if (currentAudio != null) currentAudio.volume = val;
        
        _materialPropertyBlock.SetFloat(MainTransitionSlider, val);
        propertyBlockValue = _materialPropertyBlock.GetFloat(MainTransitionSlider);
        lackMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    private void EnsureFadeOut()
    {
        fadingOutCurrentMat = true;
    }

    private Tween matTween;
    
    public SoundFXController soundFXController;
    

    private void SwitchToMaterial(Material activeMaterial)
    {
        if(activeMaterial == targetMaterial) return;

        switch (activeLackConfig.lackWorld)
        {
            case LackWorld.Crystal:
                soundFXController.Play_Lack_DissolveBlue();
                break;
            case LackWorld.Sandstone:
                soundFXController.Play_Lack_DissolveRed();
                break;
            case LackWorld.Jungle:
                soundFXController.Play_Lack_DissolveGreen();
                break;
        }
        
        Debug.Log($"Switching to {activeMaterial.name}");
        soundFXController.Play_Lack_Activate();
        materialChangeRequested = true;
        targetMaterial = activeMaterial;
    }
}