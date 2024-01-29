using UnityEngine;

public enum TargetSide : byte
{
    UNDEFINED,
    LEFT,
    RIGHT
}

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

    /// <summary>
    /// returns the value of the angle if operation succeeded, float.maxValue otherwise.you can add an angle threshold to do the test with a cone that matches the angle passed as parameter
    /// </summary>
    public static float IsTargetLeftOrRightSide(this Transform launcherTransform, Vector3 targetPos, float angleThreshold = 360)
    {
        Vector3 launcherToTargetVec = targetPos - launcherTransform.position;
        float angle = Vector3.Angle(launcherToTargetVec, launcherTransform.forward);


        if (angle <= angleThreshold && angle > float.Epsilon)
        {
            //vector that describes the enemy's position offset from the player's position along the player's left/right, up/down, and forward/back axes
            Vector3 targetLocalPosFromLauncher = launcherTransform.InverseTransformPoint(targetPos);

            //Left side of player
            if (targetLocalPosFromLauncher.x < 0)
            {
                return -angle;
            }
            //Right side of player
            else
            {
                return angle;
            }
        }

        //target isnt in the angle
        return float.MaxValue;
    }
}
