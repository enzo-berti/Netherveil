using UnityEngine;
using UnityEngine.UI;

public class BuffHUD : MonoBehaviour
{
    [SerializeField] private GameObject buffIconPrefab;
    [SerializeField] private Transform buffTransform;
    private float defaultTimeToDestroy = 4.0f;

    private void OnEnable()
    {
        Item.OnRetrieved += item => AddBuffIcon(GameResources.Get<ItemDatabase>("ItemDatabase").GetItem(item.Name).icon);
    }

    private void OnDisable()
    {
        Item.OnRetrieved -= item => AddBuffIcon(GameResources.Get<ItemDatabase>("ItemDatabase").GetItem(item.Name).icon);
    }

    public void AddBuffIcon(Texture texture)
    {
        AddBuffIcon(texture, defaultTimeToDestroy);
    }

    public void AddBuffIcon(Texture texture, float timeToDestroy)
    {
        GameObject buff = Instantiate(buffIconPrefab, buffTransform);
        buff.GetComponent<RawImage>().texture = texture;

        Destroy(buff, timeToDestroy);
    }
}
