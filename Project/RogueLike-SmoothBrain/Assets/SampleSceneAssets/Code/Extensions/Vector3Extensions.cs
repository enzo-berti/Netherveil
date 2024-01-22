using UnityEngine;
public static class Vector3Extensions
{
    public static bool IsAllValuesEqual(this Vector3 vec)
    {
        return vec.x == vec.y &&
            vec.x == vec.z &&
            vec.y == vec.z;
    }

    public static Vector3 ToAbs(this Vector3 vector)
    {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }
}
