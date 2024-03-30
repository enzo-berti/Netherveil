using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemBar : MonoBehaviour
{
    [SerializeField] private GameObject framePf;
    private List<GameObject> itemSlot = new List<GameObject>();
    [SerializeField] private ItemDatabase database;

    void Start()
    {
        Item.OnRetrieved += OnItemAdd;
        //subscribe event
    }

    private void OnItemAdd(ItemEffect itemAdd)
    {
        GameObject frame = Instantiate(framePf, transform);
        frame.GetComponentInChildren<RawImage>().texture = database.GetItem(itemAdd.Name).icon;
        itemSlot.Add(frame);

        if (transform.childCount > 5)
            DestroyImmediate(transform.GetChild(0).gameObject);
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
