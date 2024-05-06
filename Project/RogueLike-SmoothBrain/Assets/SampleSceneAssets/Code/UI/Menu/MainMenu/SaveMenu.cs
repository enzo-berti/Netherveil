using MeshUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveMenu : MonoBehaviour
{
    [SerializeField] private MeshButton[] meshButtons;
    [SerializeField] private Selectable selectable;
    private new Collider collider;

    private void Start()
    {
        collider = GetComponent<Collider>();
        DeviceManager.OnChangedToGamepad += SetSelect;
        DeviceManager.OnChangedToKB += SetUnselect;
    }
    public void EnableMenu()
    {
        collider.enabled = false;
        DeviceManager.OnChangedToGamepad += SetSelect;
        DeviceManager.OnChangedToKB += SetUnselect;
        foreach (MeshButton button in meshButtons)
        {
            button.enabled = true;
        }
    }

    public void DisableMenu()
    {
        collider.enabled = true;
        DeviceManager.OnChangedToGamepad -= SetSelect;
        DeviceManager.OnChangedToKB -= SetUnselect;
        foreach (MeshButton button in meshButtons)
        {
            button.enabled = false;
        }
    }

    private void SetSelect()
    {
        EventSystem.current.SetSelectedGameObject(selectable.gameObject);
    }
    private void SetUnselect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
