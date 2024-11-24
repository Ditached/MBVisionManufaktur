using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class GPU_Painter : MonoBehaviour
{
    public LayerMask layerMask;
    
    public float size = 1f;
    public bool paintingActive;

    public List<Transform> particles;
    
    public int TextureSize = 2048;
    public Texture2D brushTexture;
    public MeshRenderer renderer;
    
    private Camera cam;
    private static readonly int _maskTexture = Shader.PropertyToID("_MaskTexture");

    private MeshRenderer own;


    private void Start()
    {
        
        own = GetComponent<MeshRenderer>();
        cam = Camera.main;

        //Reset RT
        ResetRendertextures();
    }

    // private void CreateDefaultMaterials()
    // {
    //     defaultRenderers.ForEach(renderer =>
    //     {
    //         var newMaterial = new Material(uv2Shader);
    //         renderer.material = newMaterial;
    //     });
    // }

    [Button]
    private void ResetRendertextures()
    {
        var newRT = new RenderTexture(TextureSize, TextureSize, 0, RenderTextureFormat.ARGB32);
        newRT.enableRandomWrite = true;
        newRT.Create();
        renderer.sharedMaterial.SetTexture(_maskTexture, newRT);
       // secondRenderer.sharedMaterial.SetTexture(_dirtMask, newRT);
    }

    public void ActivateItLetsGooo()
    {
        paintingActive = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetRendertextures();
        }

        if (!paintingActive) return;

        foreach (var particle in particles)
        {

            RaycastHit hit;
            var directionToObject = (transform.position - particle.position).normalized;
            var ray = new Ray(particle.transform.position, directionToObject);

            Debug.DrawRay(ray.origin, ray.direction * 0.4f, Color.red);
            if (Physics.Raycast(ray, out hit, 0.4f, layerMask))
            {
                    var uv = hit.textureCoord;
                    Paint(uv, own.sharedMaterial.GetTexture(_maskTexture) as RenderTexture);
            }

        }
    }

    private void OnCollisionEnter(Collision other)
    {
        
        
    }

    void Paint(Vector2 uv, RenderTexture renderTexture)
    {
        uv.y = 1 - uv.y;

        int x = (int) (uv.x * renderTexture.width);
        int y = (int) (uv.y * renderTexture.height);


        RenderTexture.active = renderTexture;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, renderTexture.width, renderTexture.height, 0);
        //
        // Draw the brush texture
        Graphics.DrawTexture(
            new Rect(x - brushTexture.width * size / 2, y - brushTexture.height *size / 2, brushTexture.width * size, brushTexture.height * size),
            brushTexture);
        GL.PopMatrix();
        RenderTexture.active = null;
    }
}