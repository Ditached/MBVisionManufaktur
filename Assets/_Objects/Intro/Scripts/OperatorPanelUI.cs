using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatorPanelUI : MonoBehaviour
{
    public TMP_Text angleText;
    
    public Color activeColor;
    public Color inactiveColor;
    
    public Button WaitingButton;
    public Button RunningButton;

    public Button ConfigButton;

    private void Start()
    {
        WaitingButton.onClick.AddListener(OnWaitingBtnClick);
        RunningButton.onClick.AddListener(OnReadyBtnClick);
        
        ConfigButton.onClick.AddListener(OnConfigBtnClick);
    }

    private void OnConfigBtnClick()
    {
        UpdatePackage.configMode = !UpdatePackage.configMode;
    }

    private void OnReadyBtnClick()
    {
        UpdatePackage.globalAppState = AppState.Running;
    }

    private void OnWaitingBtnClick()
    {
        UpdatePackage.globalAppState = AppState.Waiting;
    }

    private void Update()
    {
        WaitingButton.image.color = UpdatePackage.globalAppState == AppState.Waiting ? activeColor : inactiveColor;
        RunningButton.image.color = UpdatePackage.globalAppState == AppState.Running ? activeColor : inactiveColor;
        
        ConfigButton.image.color = UpdatePackage.configMode ? activeColor : inactiveColor;

        angleText.text = $"{UpdatePackage.globalPlattformRotation:F1}°";
    }
}
