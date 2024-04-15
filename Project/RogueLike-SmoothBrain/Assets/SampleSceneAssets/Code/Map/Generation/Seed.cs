using System.Collections.Generic;

namespace Generation
{
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

        static public List<T> RandList<T>(List<T> list)
        {
            List<T> result = new List<T>();

            int iNoise = Seed.Range(0, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                int index = (i + iNoise) % list.Count;
                result.Add(list[index]);
            }

            return result;
        }
    }
}