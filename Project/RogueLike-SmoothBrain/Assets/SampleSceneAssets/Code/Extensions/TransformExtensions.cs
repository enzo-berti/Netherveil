using UnityEngine;

public static class TransformExtensions
{
    public static Vector3 Center(this Transform transform)
    {
        Vector3 sumVector = new Vector3(0f, 0f, 0f);

        foreach (Transform child in transform)
        {
            sumVector += child.position;
        }

        return sumVector / transform.childCount;
    }
}
