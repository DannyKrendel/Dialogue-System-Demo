using UnityEditor;
using UnityEngine;

public static class AssetUtils
{
    public static T FindAndLoadAsset<T>(string filter, string[] paths) where T: Object
    {
        var foundAssets = AssetDatabase.FindAssets(filter, paths);

        if (foundAssets == null || foundAssets.Length == 0)
        {
            Debug.LogError($"Asset was not found");
            return null;
        }

        var assetPath = AssetDatabase.GUIDToAssetPath(foundAssets[0]);
        
        return AssetDatabase.LoadAssetAtPath<T>(assetPath);
    }
}