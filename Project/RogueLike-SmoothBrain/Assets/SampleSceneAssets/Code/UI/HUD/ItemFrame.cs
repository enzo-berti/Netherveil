using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image background;
    [SerializeField] protected Image item;
    private string description;
    private string nameItem;
    private string state;
    private GameObject panel;

    private void Awake()
    {
        if (item != null && item.GetComponent<Selectable>() == null)
        {
            item.gameObject.AddComponent<Selectable>();
        }
    }

    public void SetFrame(Sprite _backgroundSprite, Sprite _itemSprite)
    {
        this.background.gameObject.SetActive(true);
        this.item.gameObject.SetActive(true);

        this.background.sprite = _backgroundSprite;
        this.item.sprite = _itemSprite;
        

        if (_backgroundSprite == null)
            this.background.gameObject.SetActive(false);
        if (_itemSprite == null)
            this.item.gameObject.SetActive(false);
    }

    public void SetPanel(GameObject _panel,string _name, string _description, string _state)
    {
        panel = _panel;
        nameItem = _name;
        description = _description;
        state = _state;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (description != null)
        {
            panel.SetActive(true);
            panel.GetComponentsInChildren<TextMeshProUGUI>()[0].text = nameItem;
            panel.GetComponentsInChildren<TextMeshProUGUI>()[1].text = description;
            panel.GetComponentsInChildren<TextMeshProUGUI>()[2].text = state;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (description != null)
        {
            panel.SetActive(false);
        }
    }
}
