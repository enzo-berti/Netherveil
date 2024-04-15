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
        StopAllCoroutines();
        StartCoroutine(toggle ? EasingFunctions.UpScaleCoroutine(panel, factor) : EasingFunctions.DownScaleCoroutine(panel, factor));
    }
}
