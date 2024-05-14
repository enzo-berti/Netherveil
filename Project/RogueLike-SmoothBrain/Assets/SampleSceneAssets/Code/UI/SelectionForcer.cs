using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventSystem))]
public class SelectionForcer : MonoBehaviour
{
    private GameObject previousSelect;

    private void Update()
    {
        var currentSelect = EventSystem.current.currentSelectedGameObject;
        if (currentSelect != previousSelect)
        {
            previousSelect = currentSelect;
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(previousSelect);
        }
    }
}
