using System;
using UnityEngine;

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
    public LackConfig[] lackConfigs;
}