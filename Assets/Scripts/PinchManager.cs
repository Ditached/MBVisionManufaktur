using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;

public class PinchManager : MonoBehaviour
{
    public bool MouseClickIsPinch = true;
    
    public UnityEvent<Vector3> OnPinch;
    public UnityEvent<Vector3>  OnPinchRight;
    public UnityEvent<Vector3>  OnPinchLeft;
    
    XRHandSubsystem m_HandSubsystem;
    XRHandJoint m_RightIndexTipJoint;
    XRHandJoint m_RightThumbTipJoint;
    XRHandJoint m_LeftIndexTipJoint;
    XRHandJoint m_LeftThumbTipJoint;
    bool m_ActiveRightPinch;
    bool m_ActiveLeftPinch;

    const float k_PinchThreshold = 0.02f;
    float m_ScaledThreshold;
    
    [SerializeField]
    Transform m_PolySpatialCameraTransform;


    
    private bool pinchFlag = false;

    void Start()
    {
        GetHandSubsystem();
        m_ScaledThreshold = k_PinchThreshold / m_PolySpatialCameraTransform.localScale.x;
    }
    
    void Update()
    {
        if (!CheckHandSubsystem() || Application.isEditor)
        {
            if (MouseClickIsPinch)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var positionWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var forwardOffset = 0.2f;
                    positionWorldSpace += Camera.main.transform.forward * forwardOffset;
                    OnPinch?.Invoke(positionWorldSpace);
                    OnPinchRight?.Invoke(positionWorldSpace);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    var positionWorldSpace = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var forwardOffset = 0.2f;
                    positionWorldSpace += Camera.main.transform.forward * forwardOffset; 
                    OnPinch?.Invoke(positionWorldSpace);
                    OnPinchLeft?.Invoke(positionWorldSpace);
                }
            }

            return;
        }
        
        var updateSuccessFlags = m_HandSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic);

        if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
        {
            // assign joint values
            m_RightIndexTipJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.IndexTip);
            m_RightThumbTipJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.ThumbTip);

            DetectPinch(m_RightIndexTipJoint, m_RightThumbTipJoint, ref m_ActiveRightPinch, true);
        }

        if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
        {
            // assign joint values
            m_LeftIndexTipJoint = m_HandSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
            m_LeftThumbTipJoint = m_HandSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);

            DetectPinch(m_LeftIndexTipJoint, m_LeftThumbTipJoint, ref m_ActiveLeftPinch, false);
        }
    }

    void GetHandSubsystem()
    {
        var xrGeneralSettings = XRGeneralSettings.Instance;
        if (xrGeneralSettings == null)
        {
            Debug.LogError("XR general settings not set");
        }

        var manager = xrGeneralSettings.Manager;
        if (manager != null)
        {
            var loader = manager.activeLoader;
            if (loader != null)
            {
                m_HandSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
                if (!CheckHandSubsystem())
                    return;

                m_HandSubsystem.Start();
            }
        }
    }
    
    bool CheckHandSubsystem()
    {
        if (m_HandSubsystem == null)
        {
#if !UNITY_EDITOR
                Debug.LogError("Could not find Hand Subsystem");
#endif
            
            return false;
        }

        return true;
    }
    
    void DetectPinch(XRHandJoint index, XRHandJoint thumb, ref bool activeFlag, bool right)
    {
        if (index.trackingState != XRHandJointTrackingState.None &&
            thumb.trackingState != XRHandJointTrackingState.None)
        {
            Vector3 indexPOS = Vector3.zero;
            Vector3 thumbPOS = Vector3.zero;

            if (index.TryGetPose(out Pose indexPose))
            {
                // adjust transform relative to the PolySpatial Camera transform
                indexPOS = m_PolySpatialCameraTransform.InverseTransformPoint(indexPose.position);
            }

            if (thumb.TryGetPose(out Pose thumbPose))
            {
                // adjust transform relative to the PolySpatial Camera adjustments
                thumbPOS = m_PolySpatialCameraTransform.InverseTransformPoint(thumbPose.position);
            }

            var pinchDistance = Vector3.Distance(indexPOS, thumbPOS);

            if (pinchDistance <= m_ScaledThreshold)
            {
                if (!activeFlag)
                {
                    if (right)
                    {
                        OnPinchRight?.Invoke((indexPOS + thumbPOS) / 2);
                        OnPinch?.Invoke((indexPOS + thumbPOS) / 2);
                    }
                    else
                    {
                        OnPinchLeft?.Invoke((indexPOS + thumbPOS) / 2);
                        OnPinch?.Invoke((indexPOS + thumbPOS) / 2);
                    }
                    

                    activeFlag = true;
                }
            }
            else
            {
                activeFlag = false;
            }
        }
    }


    




    bool CalculatePinch(XRHandJoint indexJoint, XRHandJoint thumbJoint, ref Vector3 midPoint)
    {
        var indexPOS = Vector3.zero;
        var thumbPOS = Vector3.zero;

        if (indexJoint.TryGetPose(out Pose indexPose))
        {
            indexPOS = indexPose.position;
        }

        if (thumbJoint.TryGetPose(out Pose thumbPose))
        {
            thumbPOS = thumbPose.position;
        }

        var distance = Vector3.Distance(indexPOS, thumbPOS);
        
        Debug.Log($"Handedness: {indexJoint.handedness} Distance: {distance}");

        if (distance <= k_PinchThreshold && !pinchFlag)
        {
            pinchFlag = true;
            var pinchMidPoint = (indexPOS + thumbPOS) / 2;
            midPoint = pinchMidPoint;
            return true;
        }
        else
        {
            pinchFlag = false;
        }

        return false;
    }


}