using UnityEngine;
using UnityEngine.UI;

public class UniversalBroadcastToggle : MonoBehaviour
{
    public static bool IsOn = false;

    public Toggle toggle;

    private void Awake()
    {
        toggle.isOn = IsOn;
        toggle.onValueChanged.AddListener(OnToggle);
    }

    private void OnToggle(bool val)
    {
        IsOn = val;
        FindFirstObjectByType<UDP_ChipSetup>().SendSetupMessage();
    }
}
