using UnityEditor;
using UnityEngine;

public static class ReplaceMeshByPrefab
{
    [MenuItem("Tools/Replace Mesh by Prefab")]
    public static void Replace()
    {
        GameObject[] objects = Selection.gameObjects;
        foreach (var item in objects)
        {
            item.name = item.name.Replace("MESH", "PREFAB");
        }
    }
}
