using UnityEngine;

public static class TransformExtensions
{
    public static Vector3 Center(this Transform transform)
    {
        Vector3 sumVector = new Vector3(0f, 0f, 0f);

        foreach (Transform child in transform)
        {
            if (child.transform.childCount > 0)
            {
                sumVector += child.transform.Center();
            }
            else
            {
                sumVector += child.transform.position;
            }
        }

        return transform.childCount == 0 ? Vector3.zero : sumVector / transform.childCount;
    }
}
