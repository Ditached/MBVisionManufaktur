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
    private MicrochipConnector _microchipConnector;

    private void Awake()
    {
        _microchipConnector = GetComponent<MicrochipConnector>();
        macAdress.text = _microchipConnector.macAdress;
    }

    private void Update()
    {
        available.color = _microchipConnector.isAvailable ? Color.green : Color.red;
        sensorActive.color = _microchipConnector.sensorIsActive ? Color.green : Color.red;
        
    }
}
