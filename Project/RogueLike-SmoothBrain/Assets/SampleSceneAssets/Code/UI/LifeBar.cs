using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField] Image filledHeart;

    [SerializeField, Range(0f, 100f)] private float lifePourcentage = 0f;
    public float LifePourcentage
    {
        get { return lifePourcentage; }
        set { lifePourcentage = Mathf.Clamp(lifePourcentage, 0, value); }
    }

    void Start()
    {
        LifePourcentage = 100f;
        filledHeart.rectTransform.transform.localScale = new Vector3(LifePourcentage / 100f, LifePourcentage / 100f, LifePourcentage / 100f);
    }

    void Update()
    {
        filledHeart.rectTransform.transform.localScale = new Vector3(LifePourcentage / 100f, LifePourcentage / 100f, LifePourcentage / 100f);
    }
}
