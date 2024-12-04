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
        {"172.20.99.155", "AVP01"},
        {"172.20.36.39", "AVP02"},
        {"172.20.155.63", "AVP03"},
        {"172.20.179.163", "AVP04"},
        {"172.20.166.222", "AVP05"},
        {"172.20.236.173", "AVP06"},
        {"172.20.55.107", "AVP07"},
        {"172.20.155.184", "AVP08"},
        {"172.20.212.222", "AVP09"},
        {"172.20.87.84", "AVP10"},
        {"172.20.123.175", "AVP11"},
        {"172.20.171.158", "AVP12"},
        {"172.20.56.101", "AVP14"},
        {"172.20.86.8", "AVP15"},
        {"172.20.242.213", "AVP16"}
        
    };
    
    public TMP_Text timeDissconnectedText;
    public TMP_Text timeConnectedText;
    public TMP_Text timePendingText;
    public CanvasGroup canvasgroup;
    
    private float timeConnected;
    private float timeDissconnected;
    private float timePending;
    
    
    [HideInInspector]
    public VisionConnection visionConnection;
    
    public static float maxPongTimePending = 2f;
    public static float maxPongTimeLost = 5f;
    public static float maxPongReallyDisconnected = 90f;
    
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

        if (diff.TotalSeconds > maxPongReallyDisconnected)
        {
            if(canvasgroup.alpha > 0.5f) transform.SetAsLastSibling();
            canvasgroup.alpha = 0.1f;
            visionIcon.color = Color.red;
            timeDissconnected += Time.deltaTime;
            
        }
        else if(diff.TotalSeconds > maxPongTimeLost)
        {
            canvasgroup.alpha = 1f;
            visionIcon.color = Color.red;
            timeDissconnected += Time.deltaTime;
        }
        else if(diff.TotalSeconds > maxPongTimePending)
        {
            canvasgroup.alpha = 1f;
            visionIcon.color = Color.yellow;
            timePending += Time.deltaTime;
        }
        else
        {
            if(canvasgroup.alpha < 0.5f) transform.SetAsFirstSibling();
            canvasgroup.alpha = 1f;
            visionIcon.color = Color.green;
            timeConnected += Time.deltaTime;
        }
        
        timeConnectedText.text = TimeSpan.FromSeconds(timeConnected).ToString(@"hh\:mm\:ss");
        timeDissconnectedText.text = TimeSpan.FromSeconds(timeDissconnected).ToString(@"hh\:mm\:ss");
        timePendingText.text = TimeSpan.FromSeconds(timePending).ToString(@"hh\:mm\:ss");
    }
}
