using UnityEngine;

public class ResetRotationBtn : MonoBehaviour
{
    public void ResetRotation()
    {
        FindFirstObjectByType<UDP_MulticastReceiver>().RequestResetRotation();
    }
}
