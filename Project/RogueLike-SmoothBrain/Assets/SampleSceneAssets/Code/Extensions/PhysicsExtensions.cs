using UnityEngine;

public static class PhysicsExtensions
{
    public static Collider[] BoxOverlap(this BoxCollider collider, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        //get the half extents with the real size by considering the scale of the object
        Vector3 adjustedHalfExtents = Vector3.Scale(collider.size * 0.5f, collider.transform.localScale.ToAbs());

        // Get the position of the BoxCollider in world space
        Vector3 center = collider.transform.TransformPoint(collider.center);

        return Physics.OverlapBox(center, adjustedHalfExtents, collider.transform.rotation, layerMask, queryTriggerInteraction);
    }

    public static Collider[] SphereOverlap(this SphereCollider collider, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        //get the radius of the object also considering the scale if scale is equal on all values
        float adjustedRadius = collider.radius;

        if (collider.transform.localScale.IsAllValuesEqual())
        {
            adjustedRadius *= Mathf.Abs(collider.transform.localScale.x);
        }
        else
        {
            Debug.LogWarning("The scale of your sphere isn't equal on all axis, this will not be taken into account for collide checks.");
        }

        // Get the position of the BoxCollider in world space
        Vector3 center = collider.transform.TransformPoint(collider.center);

        return Physics.OverlapSphere(center, adjustedRadius, layerMask, queryTriggerInteraction);
    }

    public static Collider[] CapsuleOverlap(this CapsuleCollider collider, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        // Calculate the adjusted radius and height based on the capsule's dimensions
        float adjustedRadius = Mathf.Abs(collider.radius * Mathf.Max(collider.transform.localScale.x, collider.transform.localScale.y, collider.transform.localScale.z));
        float adjustedHeight = Mathf.Abs(collider.height * collider.transform.localScale.y);

        // Get the position of the CapsuleCollider in world space
        Vector3 center = collider.transform.TransformPoint(collider.center);

        return Physics.OverlapCapsule(center - new Vector3(0f, adjustedHeight / 2f - adjustedRadius, 0f),
                                      center + new Vector3(0f, adjustedHeight / 2f - adjustedRadius, 0f),
                                      adjustedRadius, layerMask, queryTriggerInteraction);
    }
}
