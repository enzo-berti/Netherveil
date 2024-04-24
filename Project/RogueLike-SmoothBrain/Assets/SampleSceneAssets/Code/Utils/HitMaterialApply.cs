using System.Collections.Generic;
using UnityEngine;

public class HitMaterialApply : MonoBehaviour
{
    [SerializeField] private Renderer[] mRenderer;
    private Material mMaterial;

    void Awake()
    {
        mMaterial = GameResources.Get<Material>("MAT_Entity_Hit");

        if (mRenderer == null)
            mRenderer = GetComponentsInChildren<Renderer>();
        else if (mRenderer.Length == 0)
            mRenderer = GetComponentsInChildren<Renderer>();
    }

    public void SetAlpha(float alpha)
    {
        mMaterial.SetFloat("alpha", alpha);
    }

    public void EnableMat()
    {
        foreach (Renderer renderer in mRenderer)
        {
            List<Material> materials = new List<Material>(renderer.materials)
            {
                mMaterial
            };
            renderer.SetMaterials(materials);
        }
    }

    public void DisableMat()
    {
        foreach (Renderer renderer in mRenderer)
        {
            List<Material> materials = new List<Material>(renderer.materials);
            materials.RemoveAll(mat => mat.shader == mMaterial.shader);
            renderer.SetMaterials(materials);
        }
    }
}
