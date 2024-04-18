using UnityEditor;
using UnityEngine;

public static class ToolNathan

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
    [MenuItem("Tools/Delete <_Variant>")]
    public static void Delete()
    {
        GameObject[] objects = Selection.gameObjects;
;
        foreach (var item in objects)
        {
            Debug.Log(item.name);
            item.name = item.name.Replace(" Variant", "");
        }
    }
}
