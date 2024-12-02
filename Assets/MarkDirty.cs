using Unity.PolySpatial;
using UnityEngine;

public class MarkDirty : MonoBehaviour
{
    public RenderTexture rt;
    
    void Update()
    {
        PolySpatialObjectUtils.MarkDirty(rt);
        PolySpatialObjectUtils.MarkDirty(GetComponent<Renderer>());
    }
}
