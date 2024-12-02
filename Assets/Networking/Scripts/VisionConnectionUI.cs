using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VisionConnectionUI : MonoBehaviour
{
    public static Dictionary<string, string> ipToName = new Dictionary<string, string>()
    {
        {"172.20.87.84", "AVP10"}
    };
    
    public TMP_Text timeDissconnectedText;
    public TMP_Text timeConnectedText;
    public TMP_Text timePendingText;
    
    private float timeConnected;
    private float timeDissconnected;
    private float timePending;
    
    
    [HideInInspector]
    public VisionConnection visionConnection;
    
    public static float maxPongTimePending = 2f;
    public static float maxPongTimeLost = 5f;
    
    public string ip;
    [FormerlySerializedAs("udpVisionConnector")] public UDP_VisionPingReceiver udpVisionPingReceiver;
    
    public Image visionIcon;
    public TMP_Text ipText;
    public TMP_Text versionText;

    private void Update()
    {
        ipText.text = ipToName.GetValueOrDefault(ip, ip);
        versionText.text = visionConnection.buildNumber.ToString();
        
        var diff = DateTime.Now - udpVisionPingReceiver.GetLastPing(ip);
        
        if(diff.TotalSeconds > maxPongTimeLost)
        {
            visionIcon.color = Color.red;
            timeDissconnected += Time.deltaTime;
        }
        else if(diff.TotalSeconds > maxPongTimePending)
        {
            visionIcon.color = Color.yellow;
            timePending += Time.deltaTime;
        }
        else
        {
            visionIcon.color = Color.green;
            timeConnected += Time.deltaTime;
        }
        
        timeConnectedText.text = TimeSpan.FromSeconds(timeConnected).ToString(@"hh\:mm\:ss");
        timeDissconnectedText.text = TimeSpan.FromSeconds(timeDissconnected).ToString(@"hh\:mm\:ss");
        timePendingText.text = TimeSpan.FromSeconds(timePending).ToString(@"hh\:mm\:ss");
    }
}
