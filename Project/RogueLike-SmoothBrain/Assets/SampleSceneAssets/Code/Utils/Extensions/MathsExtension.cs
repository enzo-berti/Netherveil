using System.Numerics;
using UnityEngine;

public static class MathsExtension
{
    public static Vector2 GetPointOnCircle(Vector2 center, float radius)
    {
        float randomValue = Random.Range(0, 2 * Mathf.PI);
        return new Vector2(center.x + Mathf.Cos(randomValue) * radius, center.y + Mathf.Sin(randomValue) * radius);
    }

    public static Vector2 GetPointOnCone(Vector2 center, Vector2 direction, float radius, float angle)
    {
        if (direction == Vector2.zero)
        {
            Debug.LogError("Impossible to find a point because direction is a vector zero");
            return Vector2.zero;
        }
        float c = Mathf.Acos(direction.x / Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y));
        float s = Mathf.Asin(direction.y / Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y));
        float cs;
        float radAngle = angle * Mathf.Deg2Rad;
        if (s < 0)
        {
            if (c < Mathf.PI / 2)
                cs = s;
            else
                cs = Mathf.PI - s;
        }
        else
        {
            cs = c;
        }
        float randomValue = Random.Range(-radAngle + cs, radAngle + cs);
        return new Vector2(center.x + Mathf.Cos(randomValue) * radius, center.y + Mathf.Sin(randomValue) * radius);
    }

    public static float[] Resolve2ndDegree(float a, float b, float c, float wantedY)
    {
        c -= wantedY;
        float delta = b * b - 4 * a * c;
        float[] results = new float[2];
        if (delta >= 0)
        {
            results[0] = (float)(-b + Mathf.Sqrt(delta)) / (2 * a);
            results[1] = (float)(-b - Mathf.Sqrt(delta)) / (2 * a);
        }
        else if(delta == 0)
        {

        }
        else
        {
            Complex test = Complex.One;
            Debug.LogWarning("No result in Real number");
            return results;
        }
        return results;
    }

    public static float SquareFunction(float a, float b, float c, float timer)
    {
        return a * timer * timer + b * timer + c;
    }
}
