using Map.Generation;
using UnityEngine;

public class InGameManager : MonoBehaviour, ISavable
{
    public static InGameManager current;

    public Transition publicFade;

    public int altarCountRegister = 0;
    public int seedIterationRegister = 0;

    private void Awake()
    {
        current = this;

        LoadSave();
        SaveManager.onSave += Save;
    }

    /// <summary>
    /// Load all the statics variables of the game
    /// </summary>
    public void LoadSave()
    {
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