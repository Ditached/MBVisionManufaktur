using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public struct SetupMesssage
{
    public string mac;
}

public struct EventMessage
{
    public string mac;
}

public class MicrochipConnector : MonoBehaviour
{
    public static float timeSensorStayActive = 1.25f;
    public static string[] macAdresses = new[]
    {
        "38:42:A6:00:05:A8",
        "38:42:A6:00:05:72",
        "38:42:A6:00:05:AE",
        "38:42:A6:00:05:5C"
    };

    public int index;
    [Title("References")]
    public UDP_Connector udpConnector;

    public ChipState chipState;

    [Title("Microchip readonly info")]
    [ReadOnly] public string macAdress = "FF:FF:FF:FF:FF:FF";
    [ReadOnly] public bool isAvailable;
    [ReadOnly] public bool sensorIsActive;

    [HideInInspector] public UnityEvent OnTriggered;
    
    private float lastTriggered;

    private void Awake()
    {
        chipState = FindFirstObjectByType<ChipState>();
        
        macAdress = macAdresses[index];
        udpConnector.OnMessageReceived.AddListener(OnMessageReceived);
    }

    private void Update()
    {
        if(lastTriggered + timeSensorStayActive < Time.time) sensorIsActive = false;
        chipState.SetSensor(index, sensorIsActive);
    }

    private void OnMessageReceived(string msg)
    {
        try
        {
            if (msg.Contains("setup"))
            {
                var json = JsonConvert.DeserializeObject<SetupMesssage>(msg);

                if (json.mac == macAdress)
                {
                    isAvailable = true;
                }
            }
            else if (msg.Contains("event"))
            {
                var json = JsonConvert.DeserializeObject<EventMessage>(msg);

                if (json.mac == macAdress)
                {
                    Debug.Log($"Sensor {json.mac} triggered");
                    
                    sensorIsActive = true;
                    lastTriggered = Time.time;
                    
                    OnTriggered.Invoke();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void OnValidate()
    {
        macAdress = macAdresses[index];
    }
}