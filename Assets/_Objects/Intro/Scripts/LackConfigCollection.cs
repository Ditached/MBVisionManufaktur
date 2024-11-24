using System;
using System.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public enum LackWorld
{
    Sandstone,
    Crystal,
    Jungle
}

[Serializable]
public class LackConfig
{
    public string name;
    //public string macAdress;

    public Material material;
    public Color mainColor;
    public LackWorld lackWorld;
    public bool isUnassigned;
}

[CreateAssetMenu(fileName = "LackConfig", menuName = "Manufaktur/LackConfig", order = 0)]
public class LackConfigCollection : ScriptableObject
{
    
    // [HideInInspector]
    // public UnityEvent OnLackConfigChanged;
    public LackConfig[] lackConfigs;
    
}