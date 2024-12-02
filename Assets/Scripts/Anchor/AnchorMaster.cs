using System;
using System.Net;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AnchorMaster : MonoBehaviour
{
    public ARTrackedImageManager arTrackedImageManager;
    public ImageTrackingReaction imageTrackingReaction;
    
    private void Start()
    {
        var manager = FindAnyObjectByType<ARAnchorManager>();
        manager.trackablesChanged.AddListener(OnChanged);
        
        Debug.Log($"[Anchor Master] in Start we have {manager.trackables.count} anchors");
    }

    public bool IsInConfigMode;
    
    private void Update()
    {
        //var ipEndPoint = new IPEndPoint(IPAddress.Broadcast, 1200);
        IsInConfigMode = UpdatePackage.configMode;
        
        if(arTrackedImageManager == null)
            arTrackedImageManager = FindFirstObjectByType<ARTrackedImageManager>(FindObjectsInactive.Include);
        
        if(imageTrackingReaction == null)
            imageTrackingReaction = FindFirstObjectByType<ImageTrackingReaction>(FindObjectsInactive.Include);

        arTrackedImageManager.enabled = IsInConfigMode;
        imageTrackingReaction.enabled = IsInConfigMode;

        if (!IsInConfigMode)
        {
            var arTrackedImages = FindObjectsByType<AnchorPlacer>(FindObjectsSortMode.None);
            foreach (var arTrackedImage in arTrackedImages)
            {
                Destroy(arTrackedImage.gameObject);
            }
        }
    }
    
    private void OnChanged(ARTrackablesChangedEventArgs<ARAnchor> eventArgs)
    {
        foreach (var newAnchor in eventArgs.added)
        {
            Debug.Log($"[Anchor Master] - New anchor added: {newAnchor.trackableId}");
        }

        foreach (var updatedAnchor in eventArgs.updated)
        {
            Debug.Log($"[Anchor Master] - Anchor updated: {updatedAnchor.trackableId}");
        }

        foreach (var removedAnchor in eventArgs.removed)
        {
            Debug.Log($"[Anchor Master] - Anchor removed: {removedAnchor.Key}");
        }
    }
    

    public void PutAnchorAt(Vector3 position, Quaternion rotation)
    {
        var manager = FindAnyObjectByType<ARAnchorManager>();
        
        foreach (var managerTrackable in manager.trackables)
        {
            manager.TryRemoveAnchor(managerTrackable);
        }

        manager.TryAddAnchorAsync(new Pose(position, rotation));
        
    }
}
