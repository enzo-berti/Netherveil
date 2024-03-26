using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [HideInInspector]
    public List<ItemData> datas = new();

    public ItemData GetItem(string name)
    {
        return datas.Where(x => x.idName == name).FirstOrDefault();
    }
}
