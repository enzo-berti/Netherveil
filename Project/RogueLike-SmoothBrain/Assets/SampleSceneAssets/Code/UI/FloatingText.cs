using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
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
        Color newColor2 = m_TextMeshPro.color;
        newColor2.a = newColor;
        m_TextMeshPro.color = newColor2;

        if (m_TextMeshPro.alpha <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        Debug.Log("SetText");
        Debug.Log(m_TextMeshPro == null);
        m_TextMeshPro.text = text;
    }

    public void SetSize(int size)
    {
        m_TextMeshPro.fontSize = size;
    }

    public void SetColor(Color color)
    { 
        m_TextMeshPro.color = color;
    }
}
