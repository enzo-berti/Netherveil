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
        SaveData saveData = SaveManager.saveData;
        if (!saveData.hasData)
        {
            return;
        }

        if (saveData.Get<bool>(altarId.ToString()) && item != null)
        {
            Destroy(item.gameObject);
        }
    }

    public void Save(SaveData save)
    {
        SaveData saveData = SaveManager.saveData;
        saveData.Set(altarId.ToString(), GetComponentInChildren<Item>() == null);
    }
}
