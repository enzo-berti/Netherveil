using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Outline : MonoBehaviour
{
    private Renderer[] mRenderer;
    private Material mOutlineMaterial;
    [SerializeField] private Color outlineColor = Color.white;
    [SerializeField, Range(0.01f, 0.5f)] private float outlineThickness = 0.02f;

    void Start()
    {
        mOutlineMaterial = Resources.Load("OutlineShaderMat") as Material;
        mRenderer = GetComponentsInChildren<Renderer>().Where(x => x.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast")).ToArray();

        mOutlineMaterial.SetColor("_Outline_Color", outlineColor);
        mOutlineMaterial.SetFloat("_Outline_thickness", outlineThickness);
    }

    public void EnableOutline()
    {
        foreach (Renderer renderer in mRenderer)
        {
            List<Material> materials = new List<Material>(renderer.materials)
            {
                mOutlineMaterial
            };
            renderer.SetMaterials(materials);
        }
    }

    public void DisableOutline()
    {
        foreach (Renderer renderer in mRenderer)
        {
            List<Material> materials = new List<Material>(renderer.sharedMaterials);
            materials.RemoveAll(mat => mat == mOutlineMaterial);
            renderer.SetMaterials(materials);
        }
    }
}
