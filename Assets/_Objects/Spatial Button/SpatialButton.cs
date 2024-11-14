using System;
using DG.Tweening;
using TMPro;
using Unity.PolySpatial;
using Unity.PolySpatial.InputDevices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class SpatialButton : MonoBehaviour
{
    public bool IsInteractable = true;
    public UnityEvent OnClick;
    public TMP_Text text;
    
    private Tween tween;
    private MeshRenderer meshRenderer;

    public float punchScaleAnimationMultiplier = 1f;
    
    void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        EnhancedTouchSupport.Enable();
    }

    private void OnMouseDown()
    {
        if (Application.isEditor)
        {
            Click();
        }
    }

    private bool clickStarted = false;

    void Update()
    {
        GetComponent<VisionOSHoverEffect>().enabled = IsInteractable;
        if(text != null) text.alpha = IsInteractable ? 1f : 0.03f;
        
        meshRenderer.material.SetFloat("_IsInteractable", IsInteractable ? 1f : 0f);
        var activeTouches = Touch.activeTouches;
        

        if (!IsInteractable) return;
        // You can determine the number of active inputs by checking the count of activeTouches
        if (activeTouches.Count > 0)
        {
            // For getting access to PolySpatial (visionOS) specific data you can pass an active touch into the EnhancedSpatialPointerSupport()
            SpatialPointerState primaryTouchData = EnhancedSpatialPointerSupport.GetPointerState(activeTouches[0]);
            
            SpatialPointerKind interactionKind = primaryTouchData.Kind;
            GameObject objectBeingInteractedWith = primaryTouchData.targetObject;

            if (interactionKind != SpatialPointerKind.IndirectPinch) return;
            
            var parent = objectBeingInteractedWith.transform.parent;
            string parentName = parent == null ? "null" : parent.name;
            if (parent != null)
            {
                
            }

            if (objectBeingInteractedWith == gameObject)
            {
                if(primaryTouchData.phase == SpatialPointerPhase.Began)
                {
                    clickStarted = true;
                    GetComponent<AudioSource>().Play();
                }
                else if(primaryTouchData.phase == SpatialPointerPhase.Ended)
                {
                    if (clickStarted)
                    {
                        clickStarted = false;
                        Click();
                    }
                }
            }
            else
            {
                clickStarted = false;
            }
            
            
        }
    }

    private void Click()
    {
        if (!IsInteractable) return;
        
        tween?.Kill();
        tween = transform.DOPunchScale(new Vector3(0.0035f, 0.0035f, 0.0035f) * punchScaleAnimationMultiplier, 0.45f).SetEase(Ease.OutSine);
        
        Debug.Log($"[SpatialButton] Clicked on {gameObject.name}");
        OnClick.Invoke();
        
    }
}
