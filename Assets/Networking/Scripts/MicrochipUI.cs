using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MicrochipUI : MonoBehaviour
{
    public CanvasGroup globalCanvasGroup;
    public bool glowing;

    public float minBat = 3f;
    public float maxBat = 5f;

    public Image colorImage;
    public TMP_Text lackName;

    public Image batteryFill;
    public Image glowImage;
    public Image available;
    public Image sensorActive;
    public TMP_Text macAdress;
    public TMP_Text connectionTime;
    public TMP_Text totalTimeConnected;
    public TMP_Text ipAdress;
    public TMP_Text version;
    public TMP_Text batteryText;
    public Button lackChangeTriggerBtn;

    private bool _sensorActive;
    private MicrochipConnector _microchipConnector;
    private float totalTimeConnectedValueInSeconds;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnBtnClick);
        _microchipConnector = GetComponent<MicrochipConnector>();
        
        lackChangeTriggerBtn.onClick.AddListener(OnLackChangeTriggerBtnClick);
    }

    private void OnLackChangeTriggerBtnClick()
    {
        var menu = FindFirstObjectByType<LackChangeMenu>();
        
        menu.selectedMacAdress = _microchipConnector.macAdress;
        menu.Show();
    }

    private void OnBtnClick()
    {
        glowing = !glowing;
        FindFirstObjectByType<UDP_Connector>()
            .SendGlowMessage(_microchipConnector.macAdress, _microchipConnector.ipAdress, glowing);
    }

    private void Update()
    {
        globalCanvasGroup.alpha = _microchipConnector.isAvailable ? 1 : 0.66f;
        glowImage.gameObject.SetActive(glowing);

        macAdress.text = _microchipConnector.macAdress;

        available.color = _microchipConnector.isAvailable
            ? _microchipConnector.timeSinceLastMessage > MicrochipConnector.timeChipStaysConnected
                ? Color.yellow
                : Color.green
            : Color.red;

        if (_microchipConnector.isAvailable)
        {
            if (_microchipConnector.timeSinceLastMessage <= MicrochipConnector.timeChipStaysConnected)
            {
                totalTimeConnectedValueInSeconds += Time.deltaTime;
            }

            TimeSpan timeSpan = TimeSpan.FromSeconds(totalTimeConnectedValueInSeconds);
            totalTimeConnected.text = timeSpan.ToString(@"hh\:mm\:ss");
        }

        sensorActive.gameObject.SetActive(_microchipConnector.sensorIsActive);

        connectionTime.text = _microchipConnector.isAvailable
            ? _microchipConnector.timeSinceLastMessage.ToString("F2")
            : "N/A";

        ipAdress.text = _microchipConnector.ipAdress;
        version.text = _microchipConnector.firmwareVersion;
        batteryText.text = _microchipConnector.batteryStatus.ToString("F2");

        colorImage.color = _microchipConnector.GetColor();
        lackName.text = _microchipConnector.GetName();

        batteryFill.fillAmount = Mathf.InverseLerp(minBat, maxBat, _microchipConnector.batteryStatus);

        if (_microchipConnector.firmwareVersion == "") version.text = "V0.0";
    }
}