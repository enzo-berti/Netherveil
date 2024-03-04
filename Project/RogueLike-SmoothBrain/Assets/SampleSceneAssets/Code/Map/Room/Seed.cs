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
        noise++;

        return (int)((Value + noise) % (maxExclusive - minInclusive)) + minInclusive;
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