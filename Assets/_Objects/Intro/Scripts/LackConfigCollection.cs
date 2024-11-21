using System;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class LackConfig
{
    public string name;
    public string macAdress;

    public Material material;
    public Color mainColor;
    public bool isUnassigned;
}

[CreateAssetMenu(fileName = "LackConfig", menuName = "Manufaktur/LackConfig", order = 0)]
public class LackConfigCollection : ScriptableObject
{
    
    [HideInInspector]
    public UnityEvent OnLackConfigChanged;
    public LackConfig[] lackConfigs;
    
}