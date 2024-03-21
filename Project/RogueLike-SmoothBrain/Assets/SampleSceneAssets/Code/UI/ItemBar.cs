using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemBar : MonoBehaviour
{
    [SerializeField] GameObject frame;
    [SerializeField] GameObject[] itemSlot = new GameObject[5];
    Hero player;
    List<IPassiveItem> items;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Hero>();
    }

    void Update()
    {
        if (player.Inventory.PassiveItems != null)
        {
            List<IPassiveItem> passiveItems = player.Inventory.PassiveItems;
            items = passiveItems
                .Skip(Mathf.Max(0, passiveItems.Count() - 5))
                .ToList();
        }

        int c = 0;
        foreach (var item in items)
        {
            if (itemSlot[c] != null)
            {
                Destroy(itemSlot[c]);
            }
            itemSlot[c] = Instantiate(frame, transform);
            c++;
        }
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

        Debug.Log(source.Count() - N);
        return source.Skip(Mathf.Max(0, source.Count() - N));
    }
}
