using System;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class ScalePlane : MonoBehaviour
{
    public List<GameObject> objectsToScale; // List of objects to scale
    public float duration = 3f;
    // Duration of the scaling effect

    private Vector3[] ogScales;
    
    private void Awake()
    {
        ogScales = new Vector3[objectsToScale.Count];
        
        for (int i = 0; i < objectsToScale.Count; i++)
        {
            ogScales[i] = objectsToScale[i].transform.localScale;
            objectsToScale[i].transform.localScale = Vector3.zero;
        }
    }
    
    public void SetScalePercentage(float percentage)
    {
        for (var index = 0; index < objectsToScale.Count; index++)
        {
            var obj = objectsToScale[index];
            if (obj != null)
            {
                obj.transform.localScale = ogScales[index] * percentage;
            }
        }
    }
    
    /*
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
    */
}