using System;
using Unity.PolySpatial.InputDevices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SpatialCheckbox : MonoBehaviour
{
    public bool defaultOn;
    public UnityEvent<bool> OnChange;

    private bool IsChecked;
    private MeshRenderer renderer;

    private void Awake()
    {
        if (defaultOn)
        {
            IsChecked = true;
        }
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        renderer = GetComponent<MeshRenderer>();
    }
    
    private void OnMouseDown()
    {
        if (Application.isEditor)
        {
            Toggle();
        }
    }

    private void Update()
    {
        renderer.material.SetFloat("_Checked", IsChecked ? 1f : 0f);
        
        var activeTouches = Touch.activeTouches;
        
        if (activeTouches.Count > 0)
        {
            SpatialPointerState primaryTouchData = EnhancedSpatialPointerSupport.GetPointerState(activeTouches[0]);
            
            SpatialPointerKind interactionKind = primaryTouchData.Kind;
            GameObject objectBeingInteractedWith = primaryTouchData.targetObject;

            if (interactionKind != SpatialPointerKind.IndirectPinch) return;

            if (objectBeingInteractedWith == gameObject && primaryTouchData.phase == SpatialPointerPhase.Ended)
            {
                Toggle();
            }
        }
    }

    public void SetValue(bool value)
    {
        IsChecked = value;
    }

    public void Toggle()
    {
        GetComponent<AudioSource>().Play();

        IsChecked = !IsChecked;
        OnChange?.Invoke(IsChecked);
    }
}
