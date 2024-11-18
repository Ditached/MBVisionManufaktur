using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class ActionAssetOverrider : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    
    void Update()
    {
        GetComponent<InputSystemUIInputModule>().actionsAsset = inputActionAsset;
    }
}
