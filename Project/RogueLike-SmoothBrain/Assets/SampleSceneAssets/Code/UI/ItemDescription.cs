using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class ItemDescription : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    private Item item;
    private float scaleDuration = 0.25f;

    public void SetDescription(string id)
    {
        item = GetComponent<Item>();

        nameText.text = item.idItemName.SeparateAllCase();
        nameText.color = item.RarityColor;

        ItemEffect itemEffect = Assembly.GetExecutingAssembly().CreateInstance(id.GetPascalCase()) as ItemEffect;

        string descriptionToDisplay = item.Database.GetItem(id).Description;
        string[] splitDescription = descriptionToDisplay.Split(" ");
        string finalDescription = string.Empty;

        FieldInfo[] fieldOfItem = itemEffect.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        for (int i = 0; i < splitDescription.Length; i++)
        {
            if (splitDescription[i].Length > 0 && splitDescription[i][0] == '{')
            {
                string[] splitCurrent = splitDescription[i].Split('{', '}');
                string valueToFind = splitCurrent[1];

                FieldInfo valueInfo = fieldOfItem.FirstOrDefault(x => x.Name == valueToFind);

                if (valueInfo != null)
                {
                    var memberValue = valueInfo.GetValue(itemEffect);
                    splitDescription[i] = memberValue.ToString();
                }
                else
                {
                    splitDescription[i] = "N/A";
                    Debug.LogWarning($"value : {valueToFind}, has not be found");
                }
            }

            finalDescription += splitDescription[i] + " ";
        }

        descriptionText.text = finalDescription;
    }

    public void TogglePanel(bool toggle)
    {
        StopAllCoroutines();
        StartCoroutine(toggle ? EasingFunctions.UpScaleCoroutine(panel.transform, scaleDuration) : EasingFunctions.DownScaleCoroutine(panel.transform, scaleDuration));
    }
}
