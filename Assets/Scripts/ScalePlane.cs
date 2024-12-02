using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class ScalePlane : MonoBehaviour
{
    public List<GameObject> objectsToScale; // List of objects to scale
    public float duration = 3f;
    // Duration of the scaling effect

    public void ScaleAllObjects()
    {
        foreach (var obj in objectsToScale)
        {
            if (obj != null)
            {
                // Save the original scale
                Vector3 originalScale = obj.transform.localScale;

                // Set the scale to zero
                obj.transform.localScale = Vector3.zero;

                // Animate the scaling to the original size
                obj.transform.DOScale(originalScale, duration);
            }
        }
    }
}