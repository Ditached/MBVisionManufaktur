
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets;
using UnityEngine;

[InitializeOnLoad]
public class BuildVersionIncrementer
{
    static BuildVersionIncrementer()
    {
        BuildScript.buildCompleted += OnAddressableBuildCompleted;
    }

    private static void OnAddressableBuildCompleted(AddressableAssetBuildResult result)
    {
        var buildVersion = AssetDatabase.LoadAssetAtPath<BuildVersion>("Assets/BuildVersion.asset"); // Adjust path
        if (buildVersion != null)
        {
            buildVersion.buildNumber++;
            EditorUtility.SetDirty(buildVersion);
            AssetDatabase.SaveAssets();
            Debug.Log($"Build number incremented to: {buildVersion.buildNumber}");
        }
    }
}
#endif