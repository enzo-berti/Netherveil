using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    [HideInInspector]
    public List<ItemData> datas;
}
