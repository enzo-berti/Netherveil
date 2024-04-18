using UnityEditor;
using UnityEngine;

public static class ConvertUrpToToon
{
    public static Shader toonShader;

    [MenuItem("Tools/Convert Materials to ToonShader")]
    public static void ConvertMaterialsToToonShader()
    {
        string[] allMaterialGUIDs = AssetDatabase.FindAssets("t:Material");
        Shader toonShader = Shader.Find("Toon");

        foreach (string materialGUID in allMaterialGUIDs)
        {
            string materialPath = AssetDatabase.GUIDToAssetPath(materialGUID);
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

            if (material != null)
            {
                if (!material.shader.name.Contains("Toon"))
                {
                    if (toonShader != null)
                    {
                        material.shader = toonShader;
                        material.SetFloat("_BaseColor_Step", 0.25f);
                        EditorUtility.SetDirty(material);
                    }
                    else
                    {
                        Debug.LogError("ToonShader not found. Make sure the ToonShader is in your project.");
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
