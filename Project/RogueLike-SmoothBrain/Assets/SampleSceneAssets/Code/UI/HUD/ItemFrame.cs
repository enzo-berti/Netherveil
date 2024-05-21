using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image background;
    [SerializeField] protected Image item;
    private Button itemButton;
    private string description;
    private string name;
    private GameObject panel;

    private void Awake()
    {
        if (item != null && item.GetComponent<Button>() == null)
        {
            itemButton = item.gameObject.AddComponent<Button>();
        }
        else
        {
            itemButton = item.GetComponent<Button>();
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

    public void SetPanel(GameObject _panel,string _name, string _description)
    {
        panel = _panel;
        name = _name;
        description = _description;
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (description != null)
        {
            panel.SetActive(true);
            panel.GetComponentsInChildren<TextMeshProUGUI>()[0].text = name;
            panel.GetComponentsInChildren<TextMeshProUGUI>()[1].text = description;
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
