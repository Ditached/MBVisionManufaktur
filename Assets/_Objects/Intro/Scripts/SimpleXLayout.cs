using Sirenix.OdinInspector;
using UnityEngine;

public class SimpleXLayout : MonoBehaviour
{
    [SerializeField] private float spacing = 1f;

    [Button]
    public void UpdateLayout()
    {
        float currentX = 0f;
        
        foreach (Transform child in transform)
        {
            child.localPosition = Vector3.left * currentX;
            currentX += spacing;
        }
    }
}