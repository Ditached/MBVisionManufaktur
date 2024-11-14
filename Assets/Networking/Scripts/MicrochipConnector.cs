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
    public LackConfigCollection lackConfigCollection;
    public static float timeSensorStayActive = 1.25f;

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

    private void Start()
    {
        udpConnector = FindFirstObjectByType<UDP_Connector>();
        chipState = FindFirstObjectByType<ChipState>();

        macAdress = lackConfigCollection.lackConfigs[index].macAdress;
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
}