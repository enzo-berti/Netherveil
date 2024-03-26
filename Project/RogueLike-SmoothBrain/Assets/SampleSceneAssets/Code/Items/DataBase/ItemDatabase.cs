using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [HideInInspector]
    public static List<ItemData> datas = new();

    public static ItemData GetItem(string name)
    {
        return datas.Where(x => x.idName == name).FirstOrDefault();
    }
}
