using UnityEngine;

public class AnchorPlacer : MonoBehaviour
{
    public Transform center;
    
    void Start()
    {
        FindFirstObjectByType<PinchManager>().OnPinch.AddListener(OnPinch);
    }

    private void OnPinch(Vector3 pinchPos)
    {
        FindFirstObjectByType<AnchorMaster>().PutAnchorAt(center.position, center.rotation);
    }
}
