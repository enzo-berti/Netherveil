using Map.Generation;
using UnityEngine;

public class InGameManager : MonoBehaviour, ISavable
{
    public static InGameManager current;

    public Transition publicFade;

    public int altarCountRegister = 0;
    public int seedIterationRegister = 0;

    private ItemPool itemPool;
    public static ItemPool ItemPool { get { return current.itemPool; } }
    private void Awake()
    {
        current = this;
        LoadSave();
        SaveManager.onSave += Save;
        itemPool = new();
    }

    /// <summary>
    /// Load all the statics variables of the game
    /// </summary>
    public void LoadSave()
    {
        if (!SaveManager.saveData.hasData)
        {
            return;
        }

        ItemAltar.altarCount = SaveManager.saveData.altarCount;
        Seed.Iterate(SaveManager.saveData.seedIteration);

        altarCountRegister = ItemAltar.altarCount;
        seedIterationRegister = Seed.Iteration;
    }

    /// <summary>
    /// Save all the statics variables of the game
    /// </summary>
    public void Save(ref SaveData save)
    {
        save.altarCount = altarCountRegister;
        save.seedIteration = seedIterationRegister;
    }
}