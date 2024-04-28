using System.Numerics;
using UnityEngine;

public static class MathsExtension
{
    public static UnityEngine.Vector2 GetPointOnCircle(UnityEngine.Vector2 center, float radius)
    {
        float randomValue = Random.Range(0, 2 * Mathf.PI);
        return new UnityEngine.Vector2(center.x + Mathf.Cos(randomValue) * radius, center.y + Mathf.Sin(randomValue) * radius);
    }
    public static UnityEngine.Vector2 GetPointInCircle(UnityEngine.Vector2 center, float maxRadius)
    {
        float randomValue = Random.Range(0, 2 * Mathf.PI);
        float radius = Random.Range(0, maxRadius);
        return new UnityEngine.Vector2(center.x + Mathf.Cos(randomValue) * radius, center.y + Mathf.Sin(randomValue) * radius);
    }
    public static UnityEngine.Vector3 GetPointInCircle(UnityEngine.Vector3 center, float maxRadius)
    {
        UnityEngine.Vector2 center2D = new(center.x, center.z);
        UnityEngine.Vector2 point2D = GetPointInCircle(center2D, maxRadius);
        UnityEngine.Vector3 toReturn = new(point2D.x, center.y, point2D.y);
        return toReturn;
    }

    public static UnityEngine.Vector2 GetPointInCircle(UnityEngine.Vector2 center, float minRadius, float maxRadius)
    {
        float randomValue = Random.Range(0, 2 * Mathf.PI);
        float radius = Random.Range(minRadius, maxRadius);
        return new UnityEngine.Vector2(center.x + Mathf.Cos(randomValue) * radius, center.y + Mathf.Sin(randomValue) * radius);
    }
    public static UnityEngine.Vector3 GetPointInCircle(UnityEngine.Vector3 center, float minRadius, float maxRadius)
    {
        UnityEngine.Vector2 center2D = new(center.x, center.z);
        UnityEngine.Vector2 point2D = GetPointInCircle(center2D, minRadius, maxRadius);
        UnityEngine.Vector3 toReturn = new(point2D.x, center.y, point2D.y);
        Debug.Log(UnityEngine.Vector3.Distance(center, toReturn));
        return toReturn;
    }

    public static UnityEngine.Vector2 GetPointOnCone(UnityEngine.Vector2 center, UnityEngine.Vector2 direction, float radius, float angle)
    {
        if (direction == UnityEngine.Vector2.zero)
        {
            Debug.LogError("Impossible to find a point because direction is a vector zero");
            return UnityEngine.Vector2.zero;
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
        return new UnityEngine.Vector2(center.x + Mathf.Cos(randomValue) * radius, center.y + Mathf.Sin(randomValue) * radius);
    }

    public static float[] Resolve2ndDegree(float a, float b, float c, float wantedY)
    {
        c -= wantedY;
        float delta = b * b - 4 * a * c;
        float[] results = new float[2];
        if (delta == 0)
        {
            results[0] = (float)-b / (2 * a);
        }
        else if (delta > 0)
        {
            results[0] = (float)(-b + Mathf.Sqrt(delta)) / (2 * a);
            results[1] = (float)(-b - Mathf.Sqrt(delta)) / (2 * a);
        }
        else
        {
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
