using TMPro;
using UnityEngine;

public class DescriptionTab : MonoBehaviour
{
    [SerializeField] private RectTransform tabRectTransform;
    [SerializeField] private TMP_Text titleTextMesh;
    [SerializeField] private TMP_Text descriptionTextMesh;

    public void SetTab(string title, string description)
    {
        titleTextMesh.text = title;
        descriptionTextMesh.text = description;
    }

    public void OpenTab()
    {
        StartCoroutine(tabRectTransform.UpScaleCoroutine(0.1f));
    }

    public void CloseTab()
    {
        StartCoroutine(tabRectTransform.DownScaleCoroutine(0.1f));
    }
}
