using UnityEngine;

public class Seed
{
    private const int lenght = 6;
    public float Value { private set; get; } = 0f;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public float Set(float value)
    {
        return Value = value;
    }
#endif

    /// <summary>
    /// Generate new seed value
    /// </summary>
    public float Generate()
    {
        Value = 0f;
        for (int i = 0; i < lenght; i++)
        {
            Value = Value * 10 + Random.Range(1, 10);
        }

        return Value;
    }

    public int Range(int minInclusive, int maxExclusive)
    {
        return (int)(Value % (maxExclusive - minInclusive) + minInclusive);
    }

    public int Range(int minInclusive, int maxExclusive, ref int noise)
    {
        //Debug.Log("RANGE : " + minInclusive + " " + maxExclusive + " " + (noise + 1));
        //int firstModulo = (int)Value % (maxExclusive - minInclusive);
        //Debug.Log(firstModulo);
        //int randValue = firstModulo + minInclusive;
        //Debug.Log(randValue);
        //int noiseValue = (randValue + (++noise));
        //Debug.Log(noiseValue);
        //int lastModulo = noiseValue % maxExclusive;
        //Debug.Log(lastModulo);
        //return lastModulo;

        return Random.Range(minInclusive, maxExclusive); // pour le moment j'utilise le random de unity
    }

    public float Range(float minInclusive, float maxExclusive)
    {
        return Value % (maxExclusive - minInclusive) + minInclusive;
    }

    public float Range(float minInclusive, float maxExclusive, ref float noise)
    {
        return Value % (maxExclusive - minInclusive) + minInclusive;
    }
}