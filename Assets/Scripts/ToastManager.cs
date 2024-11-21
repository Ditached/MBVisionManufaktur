using Unity.VisualScripting;
using UnityEngine;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; private set; }
    public Toast prefab;
    public Transform parent;

    public float defaultLifetime = 5f;

    private void Awake()
    {
        Instance = this;
    }

    public static ToastManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = FindFirstObjectByType<ToastManager>();
        }
        
        return Instance;
    }

    public void AddToast(string text, ToastType type = ToastType.Info)
    {
        var toast = Instantiate(prefab, parent);
        toast.Init(text, defaultLifetime, type);
    }
}