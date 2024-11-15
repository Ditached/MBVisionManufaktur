using System;
using UnityEngine;
using UnityEngine.UI;

public class OperatorPanelUI : MonoBehaviour
{
    public Color activeColor;
    public Color inactiveColor;
    
    public Button WaitingButton;
    public Button RunningButton;

    private void Start()
    {
        WaitingButton.onClick.AddListener(OnWaitingBtnClick);
        RunningButton.onClick.AddListener(OnReadyBtnClick);
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
    }
}
