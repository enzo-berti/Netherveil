using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    [SerializeField] private Renderer mRenderer;
    [SerializeField] private Material mOutlineMaterial;
    [SerializeField] private Color outlineColor;
    [SerializeField, Range(0.01f, 0.5f)] private float outlineThickness = 0.02f;

    private Material[] mMaterials;

    void Start()
    {
        mMaterials = mRenderer.materials;
    }

    public void EnableOutline()
    {
        if (!ArrayContainsMaterial(mMaterials, mOutlineMaterial))
        {
            Material[] newMaterials = new Material[mMaterials.Length + 1];

            for (int i = 0; i < mMaterials.Length; i++)
            {
                newMaterials[i] = mMaterials[i];
            }

            newMaterials[mMaterials.Length] = mOutlineMaterial;
            newMaterials[mMaterials.Length].SetColor("_Outline_Color", outlineColor);
            newMaterials[mMaterials.Length].SetFloat("_Outline_thickness", outlineThickness);

            mRenderer.materials = newMaterials;
        }
    }

    public void DisableOutline()
    {
        mMaterials = ArrayRemoveMaterial(mMaterials, mOutlineMaterial);

        mRenderer.materials = mMaterials;
    }

    private bool ArrayContainsMaterial(Material[] materials, Material material)
    {
        foreach (Material mat in materials)
        {
            if (mat == material)
            {
                return true;
            }
        }
        return false;
    }

    private Material[] ArrayRemoveMaterial(Material[] materials, Material materialToRemove)
    {
        List<Material> resultList = new List<Material>(materials);

        resultList.RemoveAll(mat => mat == materialToRemove);

        return resultList.ToArray();
    }
}
