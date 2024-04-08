using UnityEngine;
public static class Vector3Extensions
{
    public static bool IsAllValuesEqual(this Vector3 vector)
    {
        return vector.x == vector.y &&
            vector.x == vector.z &&
            vector.y == vector.z;
    }

    public static Vector3 ToAbs(this Vector3 vector)
    {
        return new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
    }

    /// <param name="vector"></param>
    /// <returns>Vector2 that contains x and z which is based on camera's orientation.</returns>
    public static Vector2 ToCameraOrientedVec2(this Vector3 vector)
    {
        Vector3 tmp = (Camera.main.transform.forward * vector.z + Camera.main.transform.right * vector.x);
        return new Vector2(tmp.x, tmp.z);
    }

    /// <summary>
    /// Function to rotate a 3D Point around the Y axis
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="angleDegrees"></param>
    /// <returns></returns>
    public static Vector3 RotatePointAroundYAxis(this Vector3 vector, float angleDegrees)
    {
        // Convert angle from degrees to radians
        float angleRadians = angleDegrees * Mathf.Deg2Rad;

        // Calculate the new X and Z coordinates
        float newX = vector.x * Mathf.Cos(angleRadians) - vector.z * Mathf.Sin(angleRadians);
        float newZ = vector.x * Mathf.Sin(angleRadians) + vector.z * Mathf.Cos(angleRadians);

        // Return the rotated vector
        return new Vector3(newX, vector.y, newZ);
    }


    /// <param name="vector"></param>
    /// <param name="nbDigits"></param>
    /// <returns>Rounded values of the original vector based on the number of digits passed as parameter.</returns>
    public static Vector3 ToRound(this Vector3 vector, byte nbDigits)
    {
        float multiplier = Mathf.Pow(10, nbDigits);
        return new Vector3
        (
            Mathf.Round(vector.x * multiplier) / multiplier,
            Mathf.Round(vector.y * multiplier) / multiplier,
            Mathf.Round(vector.z * multiplier) / multiplier
        );
    }
}
