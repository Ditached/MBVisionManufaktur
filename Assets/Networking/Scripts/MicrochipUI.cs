using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicrochipUI : MonoBehaviour
{
    public Image available;
    public Image sensorActive;
    public TMP_Text macAdress;

    private bool _sensorActive;
    
    public static float interval = 1.25f;
    
    private MicrochipConnector _microchipConnector;
    private float lastTriggered;

    private void Awake()
    {
        _microchipConnector = GetComponent<MicrochipConnector>();
        _microchipConnector.OnTriggered.AddListener(OnSensorTriggered);
        
        macAdress.text = _microchipConnector.macAdress;
    }

    private void OnSensorTriggered()
    {
        _sensorActive = true;
        lastTriggered = Time.time;
    }

    private void Update()
    {
        available.color = _microchipConnector.isAvailable ? Color.green : Color.red;
        sensorActive.color = _sensorActive ? Color.green : Color.red;
        
        if(lastTriggered + interval < Time.time) _sensorActive = false;
    }
}
