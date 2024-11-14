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

    private bool _sensorActive;
    private MicrochipConnector _microchipConnector;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnBtnClick);
        _microchipConnector = GetComponent<MicrochipConnector>();
    }

    private void OnBtnClick()
    {
        glowing = !glowing;
        FindFirstObjectByType<UDP_Connector>().SendGlowMessage(_microchipConnector.macAdress, glowing);
    }

    private void Update()
    {
        glowImage.gameObject.SetActive(glowing);
        
        macAdress.text = _microchipConnector.macAdress;
        available.color = _microchipConnector.isAvailable ? Color.green : Color.red;
        sensorActive.color = _microchipConnector.sensorIsActive ? Color.green : new Color(0f, 0f, 0f, 0.02f);
    }
}
