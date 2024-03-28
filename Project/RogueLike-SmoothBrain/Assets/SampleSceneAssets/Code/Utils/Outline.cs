using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    private Renderer[] mRenderer;
    private Material mOutlineMaterial;
    [SerializeField] private Color outlineColor = Color.white;
    [SerializeField, Range(0.01f, 0.5f)] private float outlineThickness = 0.02f;

    private List<Material[]> mMaterials = new List<Material[]>();

    void Start()
    {
        mOutlineMaterial = Resources.Load("OutlineShaderMat") as Material;
        mRenderer = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in mRenderer)
        {
            mMaterials.Add(renderer.materials);
        }
    }

    public void EnableOutline()
    {
        for (int i = 0; i < mRenderer.Length; i++)
        {
            if (!ArrayContainsMaterial(mMaterials[i], mOutlineMaterial))
            {
                Material[] newMaterials = new Material[mMaterials[i].Length + 1];

                for (int j = 0; j < mMaterials[i].Length; j++)
                {
                    newMaterials[j] = mMaterials[i][j];
                }

                newMaterials[mMaterials[i].Length] = mOutlineMaterial;
                newMaterials[mMaterials[i].Length].SetColor("_Outline_Color", outlineColor);
                newMaterials[mMaterials[i].Length].SetFloat("_Outline_thickness", outlineThickness);

                mRenderer[i].materials = newMaterials;
            }
        }
    }

    public void DisableOutline()
    {
        for (int i = 0; i < mRenderer.Length; i++)
        {
            mMaterials[i] = ArrayRemoveMaterial(mMaterials[i], mOutlineMaterial);
            mRenderer[i].materials = mMaterials[i];
        }
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
