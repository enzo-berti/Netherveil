using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] TMP_Text m_TextMeshPro;
    Vector3 newPos;
    float newColor;

    void Start()
    {
        newColor = 1f;
    }

    void Update()
    {
        //grind
        newPos = transform.position;
        newPos.y += 1f * Time.deltaTime;
        transform.position = newPos;

        //fade + gris
        newColor -= 1f * Time.deltaTime;
        m_TextMeshPro.color = new Color(newColor, newColor, newColor, newColor);

        if (m_TextMeshPro.alpha <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        m_TextMeshPro.text = text;
    }

    public void SetSize(int size)
    {
        m_TextMeshPro.fontSize = size;
    }
}
