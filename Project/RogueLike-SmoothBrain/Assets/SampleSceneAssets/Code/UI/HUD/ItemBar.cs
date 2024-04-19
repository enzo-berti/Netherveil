using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemBar : MonoBehaviour
{
    [SerializeField] private GameObject framePf;
    private List<GameObject> itemSlot = new List<GameObject>();
    [SerializeField] private ItemDatabase database;
    private int maxItemDisplay = 5;
    [SerializeField] private Transform itemPassiveTransform;

    void Start()
    {
        Item.OnRetrieved += OnItemAdd;
    }

    private void OnItemAdd(ItemEffect itemAdd)
    {
        GameObject frame = Instantiate(framePf, itemPassiveTransform);
        frame.GetComponentInChildren<RawImage>().texture = database.GetItem(itemAdd.Name).icon;
        itemSlot.Add(frame);

        if (itemPassiveTransform.childCount > maxItemDisplay)
            DestroyImmediate(itemPassiveTransform.GetChild(0).gameObject);
    }

    private void OnDestroy()
    {
        Item.OnRetrieved -= OnItemAdd;
    }
}

static class ItemsExtensions
{
    static public IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
    {
        while (N > source.Count())
        {
            N--;
        }

        //Debug.Log(source.Count() - N);
        return source.Skip(Mathf.Max(0, source.Count() - N));
    }
}
