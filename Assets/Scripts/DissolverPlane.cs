using System;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteAlways]
public class DissolverPlane : MonoBehaviour
{
    public MeshRenderer[] meshRenderers;
    private MaterialPropertyBlock propertyBlock;
    
    private void Update()
    {
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_WorldPosCutoffY", transform.position.y);
        
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
