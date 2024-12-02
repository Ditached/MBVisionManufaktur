using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTrackingReaction : MonoBehaviour
{
    public GameObject prefab;
    private ARTrackedImageManager arTrackedImageManager;
    
    private void Awake()
    {
        arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();

        /*
        if (ConnectionManager.isPC)
        {
            //Create an anchor on pc 
            Instantiate(prefab);
        }
        */
    }

    void OnEnable() => arTrackedImageManager.trackablesChanged.AddListener(OnChanged);
    void OnDisable() => arTrackedImageManager.trackablesChanged.RemoveListener(OnChanged);

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        Debug.Log($"[ImageTrackingReaction] - OnChanged - {eventArgs.added.Count} added, {eventArgs.updated.Count} updated, {eventArgs.removed.Count} removed");
        
        foreach (var newImage in eventArgs.added)
        {
            AddAnchorPlacer(newImage);
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            if (updatedImage.GetComponentInChildren<AnchorPlacer>() == null)
            {
                AddAnchorPlacer(updatedImage);
            }
            // Handle updated event
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
        }
    }

    private void AddAnchorPlacer(ARTrackedImage newImage)
    {
        var spawnedObject = Instantiate(prefab);
        spawnedObject.transform.SetParent(newImage.transform);
        spawnedObject.transform.localPosition = Vector3.zero;
        spawnedObject.transform.localRotation = Quaternion.identity;
    }
}
