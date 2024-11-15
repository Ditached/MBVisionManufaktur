using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VisionConnectionUI : MonoBehaviour
{
    public static float maxPongTimePending = 2f;
    public static float maxPongTimeLost = 5f;
    
    public string ip;
    [FormerlySerializedAs("udpVisionConnector")] public UDP_VisionPingReceiver udpVisionPingReceiver;
    
    public Image visionIcon;
    public TMP_Text ipText;

    private void Update()
    {
        ipText.text = ip;
        
        var diff = DateTime.Now - udpVisionPingReceiver.GetLastPing(ip);
        
        if(diff.TotalSeconds > maxPongTimeLost)
        {
            visionIcon.color = Color.red;
        }
        else if(diff.TotalSeconds > maxPongTimePending)
        {
            visionIcon.color = Color.yellow;
        }
        else
        {
            visionIcon.color = Color.green;
        }
    }
}
