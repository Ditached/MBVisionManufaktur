using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    
    public List<MeshRenderer> renderers;
    
    void Update()
    {
        var shouldBeVisible = FindFirstObjectByType<AnchorMaster>().IsInConfigMode;
        
        foreach (var renderer in renderers)
        {
            renderer.enabled = shouldBeVisible;
        }
    }
}
