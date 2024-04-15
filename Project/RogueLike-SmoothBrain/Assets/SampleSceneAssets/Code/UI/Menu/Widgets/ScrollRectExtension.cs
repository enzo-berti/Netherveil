using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectExtension : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private float offsetY = 35f;

    private void Awake()
    {
        Selectable[] selectables = GetComponentsInChildren<Selectable>(true);
        foreach (Selectable selectable in selectables)
        {
            if (selectable.TryGetComponent(out EventTrigger trigger))
                DestroyImmediate(trigger);
                
            trigger = selectable.AddComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Select;
            entry.callback.AddListener(OnSelect);
            trigger.triggers.Add(entry);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        SnapTo(eventData.selectedObject.GetComponent<RectTransform>());
    }

    public void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        float y = scrollRect.transform.InverseTransformPoint(contentPanel.position).y - scrollRect.transform.InverseTransformPoint(target.position).y;
        contentPanel.anchoredPosition = new Vector2(0, y - offsetY);
    }
}
