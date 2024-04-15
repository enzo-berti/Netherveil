using System.Reflection;

public abstract class Quest
{
    public string idItemName;
    public string descriptionToDisplay;

    public abstract void AcceptQuest();

    static public Quest LoadClass(string name)
    {
        return Assembly.GetExecutingAssembly().CreateInstance(name.GetPascalCase()) as Quest;
    }

    //static public void RandomizeQuest(Quest quest)
    //{
    //    List<string> allQuests = new();
    //    foreach (var questInDB in quest.database.datas)
    //    {
    //        allQuests.Add(questInDB.idName);
    //    }
    //    int indexRandom = Seed.Range(0, allQuests.Count - 1);
    //    quest.idItemName = allQuests[indexRandom];
    //}

    //public void RandomizeQuest()
    //{
    //    List<string> allQuests = new();
    //    foreach (var questsInDB in database.datas)
    //    {
    //        allQuests.Add(questsInDB.idName);
    //    }
    //    int indexRandom = UnityEngine.Random.Range(0, allQuests.Count - 1);
    //    idItemName = allQuests[indexRandom];
    //}
}
