using System;
using TMPro;
using UnityEngine;

public class ChipStateButton : MonoBehaviour
{
    public int index;
    public TMP_Text text;
    public ChipState chipState;

    private void Awake()
    {
        GetComponent<SpatialButton>().OnClick.AddListener(OnClick);
        text.text = index.ToString();
    }

    private void OnClick()
    {
        chipState.SetOnlyThisSensor(index);
    }

    public void OnValidate()
    {
        text.text = index.ToString();
    }
}
