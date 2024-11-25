using UnityEngine;

[CreateAssetMenu(fileName = "BuildVersion", menuName = "Custom/Build Version")]
public class BuildVersion : ScriptableObject
{
    public ushort buildNumber = 0;
}