using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

/*
 * {"setup":1,"wd":1,"vbat":4.956284046,"ver":"V2.0.2","pcb":"V1.1A","mac":"38:42:A6:00:05:43","PrimKey":"7R10"}
 */

public struct SetupMesssage
{
    public string mac;
    public string ver;
    public string pcb;
    public string PrimKey;
}

public struct PingMessage
{
    public string mac;
}

public struct HalStatusMsg
{
    public string mac;
    public string wd;
    public string conf;
    public string drawer;
}

//{"event":"hal activated","mac":"38:42:A6:00:05:43","wd":0,"conf":1,"drawer":1,"vbat":4.956284046}
public struct HalActivatedMessage
{
    public string mac;
    public string wd;
    public string conf;
    public string drawer;
}

public struct HalDeactivatedMessage
{
    public string mac;
    public string wd;
    public string conf;
    public string drawer;
}

public struct StatusMessage
{
    public string mac;
    public float vbat;
}

public class MicrochipConnector : MonoBehaviour
{
    public bool overwriteSensorActive;
    
    public LackConfigCollection lackConfigCollection;
    public static float timeSensorStayActive = 10f; //This is a safety timeout, e.g. sensor is over the magnet but battery runs out
    public static float timeChipStaysConnected = 20f;

    [Title("References")] public UDP_Connector udpConnector;

    public ChipState chipState;

    [Title("Microchip readonly info")] [ReadOnly]
    public string macAdress = "FF:FF:FF:FF:FF:FF";
    [ReadOnly] public string ipAdress = "";

    [ReadOnly] public bool isAvailable;
    [ReadOnly] public bool sensorIsActive;
    [ReadOnly] public float lastMessageReceived;
    [ReadOnly] public float timeSinceLastMessage;
    [ReadOnly] public string firmwareVersion = "V0.0";
    [ReadOnly] public float timeSinceLastHalStatus;
    [ReadOnly] public float batteryStatus;

    private float lastHalStatusReceived;
    
    private int index;

    private void Start()
    {
        udpConnector = FindFirstObjectByType<UDP_Connector>();
        chipState = FindFirstObjectByType<ChipState>();

        GetIndexBasedOnMac();
        
        udpConnector.OnMessageReceivedWithEndPoint.AddListener(OnMessageReceived);
        MacAddrProvider.instance.OnMacChanged.AddListener(OnLackConfigChanged);
    }

    private void GetIndexBasedOnMac()
    {
        index = MacAddrProvider.GetIndex(macAdress);
    }

    private void OnLackConfigChanged()
    {
        var prevIndex = index;
        
        GetIndexBasedOnMac();
        
        if (prevIndex != index)
        {
            Debug.Log($"Index changed from {prevIndex} to {index}");
        }
    }

    public string GetName()
    {
        var name = lackConfigCollection.lackConfigs[index].name;
        return name;
    }

    public Color GetColor()
    {
        var color = lackConfigCollection.lackConfigs[index].mainColor;
        return color;
    }

    private void Update()
    {
        if (sensorIsActive && lastHalStatusReceived + timeSensorStayActive < Time.time) sensorIsActive = false;
        chipState.SetSensor(index, sensorIsActive || overwriteSensorActive);

        timeSinceLastMessage = Time.time - lastMessageReceived;
        timeSinceLastHalStatus = Time.time - lastHalStatusReceived;
    }

    private void OnMessageReceived(string msg, IPEndPoint endPoint)
    {
        try
        {
            if (msg.Contains("setup"))
            {
                var json = JsonConvert.DeserializeObject<SetupMesssage>(msg);

                if (json.mac == macAdress)
                {
                    isAvailable = true;
                    lastMessageReceived = Time.time;
                    ipAdress = endPoint.Address.ToString();
                    firmwareVersion = json.ver;
                }
            }
            else if (msg.Contains("ping"))
            {
                var json = JsonConvert.DeserializeObject<PingMessage>(msg);
                
                if (json.mac == macAdress)
                {
                    lastMessageReceived = Time.time;
                }
            }
            else if (msg.Contains("hal status"))
            {
                var json = JsonConvert.DeserializeObject<HalStatusMsg>(msg);

                if (json.mac == macAdress)
                {
                    Debug.Log($"Sensor {json.mac} triggered. WD: {json.wd}, Conf: {json.conf}, Drawer: {json.drawer}");
                    lastHalStatusReceived = Time.time;
                }
            }
            else if (msg.Contains("hal activated"))
            {
                var json = JsonConvert.DeserializeObject<HalActivatedMessage>(msg);
                
                if (json.mac == macAdress)
                {
                    Debug.Log($"Sensor {json.mac} activated. WD: {json.wd}, Conf: {json.conf}, Drawer: {json.drawer}");
                    lastHalStatusReceived = Time.time;
                    sensorIsActive = true;
                }
            }
            else if(msg.Contains("hal deactivated"))
            {
                var json = JsonConvert.DeserializeObject<HalDeactivatedMessage>(msg);
                
                if (json.mac == macAdress)
                {
                    Debug.Log($"Sensor {json.mac} deactivated. WD: {json.wd}, Conf: {json.conf}, Drawer: {json.drawer}");
                    sensorIsActive = false;
                }
            } else if (msg.Contains("status") && msg.Contains("vbat"))
            {
                var json = JsonConvert.DeserializeObject<StatusMessage>(msg);

                if (json.mac == macAdress)
                {
                    lastMessageReceived = Time.time;
                    batteryStatus = json.vbat;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}