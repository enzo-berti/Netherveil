using Map.Generation;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour, ISavable
{
    public static InGameManager current;

    public Transition publicFade;

    public int altarCountRegister = 0;
    public int seedIterationRegister = 0;
    public List<List<string>> itemsPoolRegister;

    private ItemPool itemPool;
    public static ItemPool ItemPool { get { return current.itemPool; } }
    private void Awake()
    {
        current = this;
        LoadSave();
        SaveManager.onSave += Save;
        itemPool = new();
    }

    public void RegisterGameValues()
    {
        seedIterationRegister = Seed.Iteration;
        altarCountRegister = ItemAltar.altarCount;
        itemsPoolRegister = ItemPool.itemsPerTier;
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
        itemPool.itemsPerTier = SaveManager.saveData.itemsPool;

        altarCountRegister = ItemAltar.altarCount;
        seedIterationRegister = Seed.Iteration;
        itemsPoolRegister = itemPool.itemsPerTier;
    }

    /// <summary>
    /// Save all the statics variables of the game
    /// </summary>
    public void Save(ref SaveData save)
    {
        save.altarCount = altarCountRegister;
        save.seedIteration = seedIterationRegister;
        save.itemsPool = itemsPoolRegister;
    }
}