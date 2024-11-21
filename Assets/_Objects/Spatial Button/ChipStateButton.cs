using System;
using TMPro;
using UnityEngine;

public class ChipStateButton : MonoBehaviour
{
    public LackConfigCollection lackConfigCollection;
    public int index;
    public TMP_Text text;
    public ChipState chipState;

    private void Awake()
    {
        GetComponent<SpatialButton>().OnClick.AddListener(OnClick);
        text.text = index.ToString();
        
        if (lackConfigCollection.lackConfigs[index].material != null)
        {
            GetComponent<MeshRenderer>().material = lackConfigCollection.lackConfigs[index].material;
        }
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
