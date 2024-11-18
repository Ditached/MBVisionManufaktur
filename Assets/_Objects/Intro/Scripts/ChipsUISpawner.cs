using System;
using UnityEngine;

public class ChipsUISpawner : MonoBehaviour
{
    public LackConfigCollection lackConfigCollection;
    public GameObject microchipUIPrefab;

    private void Awake()
    {
        for (var index = 0; index < lackConfigCollection.lackConfigs.Length; index++)
        {
            var lackConfig = lackConfigCollection.lackConfigs[index];
            
            var microchipUI = Instantiate(microchipUIPrefab, transform);
            microchipUI.GetComponent<MicrochipConnector>().lackConfigCollection = lackConfigCollection;
            microchipUI.GetComponent<MicrochipConnector>().index = index;
        }
    }
}
