using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemFrame : MonoBehaviour
{
    [SerializeField] protected Image background;
    [SerializeField] protected Image item;

    public void SetFrame(Sprite background, Sprite item)
    {
        this.background.gameObject.SetActive(true);
        this.item.gameObject.SetActive(true);

        this.background.sprite = background;
        this.item.sprite = item;

        if (background == null)
            this.background.gameObject.SetActive(false);
        if (item == null)
            this.item.gameObject.SetActive(false);
    }
}
