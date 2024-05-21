using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class ItemSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string description;
    private new string name;
    private string state;
    private GameObject panel;
    private RectTransform rectTransform => panel.transform as RectTransform;
    private Coroutine routine;
    private float durationScale = 0.1f;

    private void OnDisable()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            rectTransform.localScale = Vector3.zero;
        }
    }

    public void SetPanel(GameObject _panel, string _name, string _description, string _state)
    {
        panel = _panel;
        name = _name;
        description = _description;
        state = _state;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // if no description register
        if (description == null)
            return;

        // setters
        panel.GetComponentsInChildren<TextMeshProUGUI>()[0].text = name;
        panel.GetComponentsInChildren<TextMeshProUGUI>()[1].text = description;
        panel.GetComponentsInChildren<TextMeshProUGUI>()[2].text = state;

        // Rect Position
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.pivot = Vector2.zero;
        rectTransform.localScale = Vector3.one;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out Vector2 mousePosition);

        if (mousePosition.y > Screen.height / 2.0f)
            rectTransform.pivot = Vector2.up;

        rectTransform.anchoredPosition = mousePosition;
        rectTransform.localScale = Vector3.zero;

        // scale routine
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(rectTransform.UpScaleCoroutine(durationScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // if no description register
        if (description == null)
            return;

        // scale routine
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(rectTransform.DownScaleCoroutine(durationScale));
    }
}
