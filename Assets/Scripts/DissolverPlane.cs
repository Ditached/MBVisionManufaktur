using System;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class DissolverPlane : MonoBehaviour
{
    
    [ColorUsage(true, true)]
    public Color hdrEdgeColor = new Color(0.5f, 0.5f, 0.5f, 1);
    public float edge = 0.05f;
    public MeshRenderer[] meshRenderers;
    private MaterialPropertyBlock propertyBlock;
    
    private void Update()
    {
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_WorldPosCutoffY", transform.position.y);
        propertyBlock.SetFloat("_Edge", edge);
        propertyBlock.SetColor("_EdgeColor", hdrEdgeColor);
        
        foreach (var meshRenderer in meshRenderers)
        {
            meshRenderer.SetPropertyBlock(propertyBlock);
        }
    }

    [Button]
    private void FindDissolvers()
    {
        var parent = transform.parent;
        meshRenderers = parent.GetComponentsInChildren<MeshRenderer>();
    }
}
