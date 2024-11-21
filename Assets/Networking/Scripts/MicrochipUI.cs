using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicrochipUI : MonoBehaviour
{
    public bool glowing;

    public Image glowImage;
    public Image available;
    public Image sensorActive;
    public TMP_Text macAdress;
    public TMP_Text connectionTime;
    public TMP_Text totalTimeConnected;
    public TMP_Text ipAdress;
    public TMP_Text version;
    public TMP_Text batteryText;

    private bool _sensorActive;
    private MicrochipConnector _microchipConnector;
    private float totalTimeConnectedValueInSeconds;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnBtnClick);
        _microchipConnector = GetComponent<MicrochipConnector>();
    }

    private void OnBtnClick()
    {
        glowing = !glowing;
        FindFirstObjectByType<UDP_Connector>().SendGlowMessage(_microchipConnector.macAdress, _microchipConnector.ipAdress, glowing);
    }

    private void Update()
    {
        glowImage.gameObject.SetActive(glowing);

        macAdress.text = _microchipConnector.macAdress;

        available.color = _microchipConnector.isAvailable
            ? _microchipConnector.timeSinceLastMessage > MicrochipConnector.timeChipStaysConnected
                ? Color.yellow
                : Color.green
            : Color.red;

        totalTimeConnected.enabled = _microchipConnector.isAvailable;
        
        if (_microchipConnector.isAvailable)
        {
            if (_microchipConnector.timeSinceLastMessage < MicrochipConnector.timeChipStaysConnected)
            {
                totalTimeConnectedValueInSeconds += Time.deltaTime;
            }
            
            TimeSpan timeSpan = TimeSpan.FromSeconds(totalTimeConnectedValueInSeconds);
            totalTimeConnected.text = timeSpan.ToString(@"hh\:mm\:ss");
        }

        sensorActive.color = _microchipConnector.sensorIsActive ? Color.green : new Color(0f, 0f, 0f, 0.02f);
        
        connectionTime.text = _microchipConnector.isAvailable
            ? _microchipConnector.timeSinceLastMessage.ToString("F2")
            : "N/A";
        
        ipAdress.text = _microchipConnector.ipAdress;
        version.text = _microchipConnector.firmwareVersion;
        batteryText.text = _microchipConnector.batteryStatus.ToString("F2");
        
        if (_microchipConnector.firmwareVersion == "") version.text = "V0.0";
    }
}