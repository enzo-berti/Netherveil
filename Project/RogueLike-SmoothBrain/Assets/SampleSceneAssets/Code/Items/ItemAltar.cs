using UnityEngine;

public class ItemAltar : MonoBehaviour, ISavable
{
    public static int altarCount = 0;
    private int altarId = 0;

    private Item item = null;

    private void Awake()
    {
        altarId = altarCount++;
        item = GetComponentInChildren<Item>();

        LoadSave();
        SaveManager.onSave += Save;
    }

    private void OnDestroy()
    {
        SaveManager.onSave -= Save;
    }

    public void LoadSave()
    {
        if (SaveManager.saveData.altarCleareds.Contains(altarId))
        {
            Destroy(item.gameObject);
        }
    }

    public void Save(ref SaveData save)
    {
        if (GetComponentInChildren<Item>() == null)
        {
            save.altarCleareds.Add(altarId);
        }
    }
}
