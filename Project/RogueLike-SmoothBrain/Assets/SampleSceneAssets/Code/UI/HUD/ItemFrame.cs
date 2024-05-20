using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ItemFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image background;
    [SerializeField] protected Image item;
    private Button itemButton;
    private Item itemInstance; // Reference to the Item instance

    public UnityEvent onPointerEnter;
    public UnityEvent onPointerExit;

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

    public void SetFrame(Sprite backgroundSprite, Sprite itemSprite)
    {
        this.background.gameObject.SetActive(true);
        this.item.gameObject.SetActive(true);

        this.background.sprite = backgroundSprite;
        this.item.sprite = itemSprite;

        if (backgroundSprite == null)
            this.background.gameObject.SetActive(false);
        if (itemSprite == null)
            this.item.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnter?.Invoke();
        if (itemInstance != null)
        {
            //Debug.Log(itemInstance.GetComponent<ItemDescription>().TogglePanel(true));
            Debug.Log("show Description");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onPointerExit?.Invoke();
        if (itemInstance != null)
        {
            Debug.Log("Hide description");
        }
    }
}
