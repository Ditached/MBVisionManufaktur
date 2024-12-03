using System;
using UnityEngine;

public class NotesShowerHider : MonoBehaviour
{
    public MeshRenderer renderer;
    public GameObject notes;


    private void Awake()
    {
        GetComponent<SpatialCheckbox>().OnChange.AddListener(OnChange);
        OnChange(GetComponent<SpatialCheckbox>().defaultOn);
    }

    private void OnChange(bool state)
    {
        notes.SetActive(state);
        renderer.enabled = state;
    }
}
