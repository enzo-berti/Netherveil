using System.Collections;
using TMPro;
using UnityEngine;

public class ItemDescription : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descriptionText;
    Item item;
    float factor;
    void Start()
    {
        factor = Time.deltaTime * 2f;
        item = GetComponent<Item>();
        nameText.text = item.idItemName.SeparateAllCase();
        nameText.color = item.RarityColor;
        descriptionText.text = item.descriptionToDisplay;
    }

    public void TogglePanel(bool toggle)
    {
        if(toggle)
        {
            StopAllCoroutines();
            StartCoroutine(UpScaleCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(DownScaleCoroutine());
        }
    }

    IEnumerator UpScaleCoroutine()
    {
        while(panel.transform.localScale.x < 1f)
        {
            Vector3 scale = panel.transform.localScale;
            scale += new Vector3(factor, factor, factor);
            scale = new Vector3(Mathf.Min(scale.x, 1f), Mathf.Min(scale.y, 1f), Mathf.Min(scale.z, 1f));
            panel.transform.localScale = scale;
            yield return null;
        }
    }

    IEnumerator DownScaleCoroutine()
    {
        while (panel.transform.localScale.x > 0f)
        {
            Vector3 scale = panel.transform.localScale;
            scale -= new Vector3(factor, factor, factor);
            scale = new Vector3(Mathf.Max(scale.x, 0f), Mathf.Max(scale.y, 0f), Mathf.Max(scale.z, 0f));
            panel.transform.localScale = scale;
            yield return null;
        }
    }
}
