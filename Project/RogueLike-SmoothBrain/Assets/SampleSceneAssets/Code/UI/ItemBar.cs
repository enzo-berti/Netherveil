using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemBar : MonoBehaviour
{
    [SerializeField] GameObject frame;
    [SerializeField] GameObject[] itemSlot = new GameObject[5];
    GameObject player;
    List<IPassiveItem> items;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else if (player.TryGetComponent(out Hero hero) && hero.Inventory.PassiveItems != null)
        {
            List<IPassiveItem> passiveItems = hero.Inventory.PassiveItems;
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
