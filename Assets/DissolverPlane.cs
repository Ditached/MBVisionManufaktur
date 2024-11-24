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
        var amountOfDissolver = 0;
        var rocks = FindObjectsByType<FloatyRock>(FindObjectsSortMode.None);
        
        amountOfDissolver += rocks.Length;
        
        meshRenderers = new MeshRenderer[amountOfDissolver];
        
        for (var i = 0; i < rocks.Length; i++)
        {
            meshRenderers[i] = rocks[i].GetComponent<MeshRenderer>();
        }
        
    }
}
