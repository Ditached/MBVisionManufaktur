using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-100)] 
public class MacAddrProvider : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnMacChanged;
    public static MacAddrProvider instance;
    
    public LackConfigCollection lackConfigCollection;
    private List<string> macAdresses = new List<string>();
    
    private string savePath => Application.streamingAssetsPath + "/macAdresses.json";
    
    private void Awake()
    {
        LoadJson();
        instance = this;
    }
    
    public static string GetMac(int index)
    {
        return instance.GetMacForIndex(index);
    }
    
    public static int GetIndex(string mac)
    {
        return instance.macAdresses.IndexOf(mac);
    }

    public void SwapMacs(int index1, int index2)
    {
        var mac1 = GetMacForIndex(index1);
        var mac2 = GetMacForIndex(index2);
        
        SetMacForIndex(index1, mac2);
        SetMacForIndex(index2, mac1);
        
        OnMacChanged.Invoke();
        SaveJson();
    }

    private void SetMacForIndex(int index, string mac)
    {
        macAdresses[index] = mac;
        OnMacChanged.Invoke();
        SaveJson();
    }

    public string GetMacForIndex(int index)
    {
        if (index < 0 || index >= macAdresses.Count)
        {
            return "00:00:00:00:00:00";
        }

        return macAdresses[index];
    }

    private void SaveJson()
    {
        var json = JsonConvert.SerializeObject(macAdresses, Formatting.Indented);
        File.WriteAllText(savePath, json);
    }
    
    private void LoadJson()
    {
        if (!File.Exists(savePath))
        {
            return;
        }
        
        var json = File.ReadAllText(savePath);
        macAdresses = JsonConvert.DeserializeObject<List<string>>(json);

        if (macAdresses.Count != lackConfigCollection.lackConfigs.Length)
        {
            Debug.LogWarning("MacAdresses count does not match lackConfigs count!");

            if (lackConfigCollection.lackConfigs.Length > macAdresses.Count)
            {
                var diff = lackConfigCollection.lackConfigs.Length - macAdresses.Count;
                
                for (var i = 0; i < diff; i++)
                {
                    macAdresses.Add("00:00:00:00:00:00");
                }
                
                SaveJson();
            }
            else
            {
                Debug.LogError("MacAdresses count is higher than lackConfigs count, adjust JSON!");
                var diff = macAdresses.Count - lackConfigCollection.lackConfigs.Length;
                
                for (var i = 0; i < diff; i++)
                {
                    macAdresses.RemoveAt(macAdresses.Count - 1);
                }
            }
        }
    }
   
}
