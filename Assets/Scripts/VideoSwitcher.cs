using System;
using System.Collections;
using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.Video;

public class VideoSwitcher : MonoBehaviour
{
    public Texture sandstoneVideo;
    public Texture crystalVideo;
    public Texture jungleVideo;
    
    public ParticleSystem[] particleSystemSandstone;
    public ParticleSystem[] particleSystemCrystal;
    public ParticleSystem[] particleSystemJungle;
    
    public float particlesTime = 3f;

    private Renderer material;
    private MeshRenderer meshRenderer;


    private void Awake()
    {
        material = GetComponent<Renderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    private void OnEnable()
    {
        var lackSwitcher = FindObjectOfType<LackSwitcher>();
        lackSwitcher.OnWorldChanged += HandleWorldChanged;
        lackSwitcher.NoWorldActive += HandleNoWorldActive;
    }

    private void OnDisable()
    {
        var lackSwitcher = FindObjectOfType<LackSwitcher>();
    
        if (lackSwitcher != null)
        {
            lackSwitcher.OnWorldChanged -= HandleWorldChanged;
            lackSwitcher.NoWorldActive -= HandleNoWorldActive;
        }
    }

    private void HandleNoWorldActive()
    {
        if (meshRenderer.enabled)
        {
            meshRenderer.enabled = false;
        }

        /*videoPlayer.Stop();
        videoPlayer.Clip = null;*/
    }
    
    public void PlayForSeconds(ParticleSystem[] particleSystems, float seconds)
    {
        foreach (var ps in particleSystems)
        {
            ps.Play();
        }
        StartCoroutine(StopAfterSeconds(particleSystems, seconds));
    }
    
    private IEnumerator StopAfterSeconds(ParticleSystem[] particleSystems, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        foreach (var ps in particleSystems)
        {
            ps.Stop();
        }
    }


    //TODO @Seppi, Change this
    // Particle system can be destroyed error
    private void HandleWorldChanged(LackWorld world)
    {
        try
        {
            if (!meshRenderer.enabled)
            {
                meshRenderer.enabled = true;
            }
        
            switch (world)
            {
                case LackWorld.Sandstone:
                    material.material.SetTexture("_BaseMap", sandstoneVideo);
                    PlayForSeconds(particleSystemSandstone, particlesTime);
                    break;
                case LackWorld.Crystal:
                    material.material.SetTexture("_BaseMap", crystalVideo);
                    PlayForSeconds(particleSystemCrystal, particlesTime);
                    break;
                case LackWorld.Jungle:
                    material.material.SetTexture("_BaseMap", jungleVideo);
                    PlayForSeconds(particleSystemJungle, particlesTime);
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("error... Seppi: I will change this anyway");
        }
        
    }
}