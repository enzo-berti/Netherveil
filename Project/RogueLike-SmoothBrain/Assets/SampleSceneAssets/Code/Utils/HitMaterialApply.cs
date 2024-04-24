using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMaterialApply : MonoBehaviour
{
    [SerializeField] private Renderer[] mRenderer;
    private Material mMaterial;
    private Coroutine routine;
    Func<float, float> defaultEasing = e => e;

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

    public void SetAlpha(float from, float to, float duration)
    {
        SetAlpha(from, to, duration, defaultEasing);
    }

    public void SetAlpha(float from, float to, float duration, Func<float, float> easingFunction)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(SetAlphaRoutine(from, to, duration, easingFunction));
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

    private IEnumerator SetAlphaRoutine(float from, float to, float duration, Func<float, float> easingFunction)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            elapsed = Mathf.Max(elapsed + Time.deltaTime, 0.0f);
            float factor = elapsed / duration;
            float ease = easingFunction(factor);
            float result = Mathf.Lerp(from, to, factor);

            SetAlpha(result);

            yield return null;
        }
    }
}
