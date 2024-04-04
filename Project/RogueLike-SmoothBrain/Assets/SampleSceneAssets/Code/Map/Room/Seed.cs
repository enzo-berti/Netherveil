static public class Seed
{
    static private System.Random randGen = new System.Random();

    static public void SetSeed(int seed)
    {
        randGen = new System.Random(seed);
    }

    static public int Range(int minInclusive, int maxInclusive)
    {
        return randGen.Next(minInclusive, maxInclusive);
    }
}