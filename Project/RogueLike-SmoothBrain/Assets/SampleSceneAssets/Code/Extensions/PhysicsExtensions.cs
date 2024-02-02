using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public static class PhysicsExtensions
{
    static float CAST_THRESHOLD = 0.01f;

    /// <summary>
    /// Used To Get Colliders from an overlap from an existing box collider.
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="layerMask"></param>
    /// <param name="queryTriggerInteraction"></param>
    /// <returns></returns>
    public static Collider[] BoxOverlap(this BoxCollider collider, int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        //get the half extents with the real size by considering the scale of the object
        Vector3 adjustedHalfExtents = Vector3.Scale(collider.size * 0.5f, collider.transform.localScale.ToAbs());

        // Get the position of the BoxCollider in world space
        Vector3 center = collider.transform.TransformPoint(collider.center);

        return Physics.OverlapBox(center, adjustedHalfExtents, collider.transform.rotation, layerMask, queryTriggerInteraction);
    }

    /// <summary>
    /// Used to get colliders from a box cast from an existing box collider.
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="targetTag"></param>
    /// <param name="tagToIgnore"></param>
    /// <param name="layerMask"></param>
    /// <param name="queryTriggerInteraction"></param>
    /// <returns></returns>
    public static List<Collider> BoxCastAll(this BoxCollider collider, string targetTag, string tagToIgnore = "", int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        //get the half extents with the real size by considering the scale of the object
        Vector3 adjustedHalfExtents = Vector3.Scale(collider.size * 0.5f, collider.transform.localScale.ToAbs());

        // Get the position of the BoxCollider in world space
        Vector3 center = collider.transform.TransformPoint(collider.center);

        RaycastHit[] hits = Physics.BoxCastAll(center, adjustedHalfExtents, collider.gameObject.transform.forward,
            collider.transform.rotation, CAST_THRESHOLD, layerMask, queryTriggerInteraction);

        return GetCollidersFromCast(hits, collider.transform, targetTag, tagToIgnore);
    }

    /// <summary>
    /// Used to get colliders from a sphere overlap from an existing sphere collider.
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="layerMask"></param>
    /// <param name="queryTriggerInteraction"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Used to get colliders from a sphere cast from an existing sphere collider.
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="targetTag"></param>
    /// <param name="tagToIgnore"></param>
    /// <param name="layerMask"></param>
    /// <param name="queryTriggerInteraction"></param>
    /// <returns></returns>
    public static List<Collider> SphereCastAll(this SphereCollider collider, string targetTag, string tagToIgnore = "", int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
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

        RaycastHit[] hits = Physics.SphereCastAll(center, adjustedRadius, collider.gameObject.transform.forward, CAST_THRESHOLD, layerMask, queryTriggerInteraction);

        return GetCollidersFromCast(hits, collider.transform, targetTag, tagToIgnore);
    }

    /// <summary>
    /// Used to get colliders from a capsule overlap from an existing capsule collider.
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="layerMask"></param>
    /// <param name="queryTriggerInteraction"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Used to get colliders from a capsule cast from an existing capsule collider.
    /// </summary>
    /// <param name="collider"></param>
    /// <param name="targetTag"></param>
    /// <param name="tagToIgnore"></param>
    /// <param name="layerMask"></param>
    /// <param name="queryTriggerInteraction"></param>
    /// <returns></returns>
    public static List<Collider> CapsuleCastAll(this CapsuleCollider collider, string targetTag, string tagToIgnore = "", int layerMask = -1, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
    {
        // Calculate the adjusted radius and height based on the capsule's dimensions
        float adjustedRadius = Mathf.Abs(collider.radius * Mathf.Max(collider.transform.localScale.x, collider.transform.localScale.y, collider.transform.localScale.z));
        float adjustedHeight = Mathf.Abs(collider.height * collider.transform.localScale.y);

        // Get the position of the CapsuleCollider in world space
        Vector3 center = collider.transform.TransformPoint(collider.center);

        RaycastHit[] hits = Physics.CapsuleCastAll(center - new Vector3(0f, adjustedHeight / 2f - adjustedRadius, 0f),
            center + new Vector3(0f, adjustedHeight / 2f - adjustedRadius, 0f)
            , adjustedRadius, collider.gameObject.transform.forward, CAST_THRESHOLD, layerMask, queryTriggerInteraction);


        return GetCollidersFromCast(hits, collider.transform, targetTag, tagToIgnore);
    }

    /// <summary>
    /// Find all collider toucing the vision cone.
    /// </summary>
    /// <param name="center">The start position of the cone.</param>
    /// <param name="angle">Cone's angle.</param>
    /// <param name="range">Cone's range.</param>
    /// <param name="forward">Where the cone is facing.</param>
    /// <returns></returns>
    public static Collider[] OverlapVisionCone(Vector3 center, float angle, float range, Vector3 forward, int layer = -1)
    {
        Collider[] result = Physics.OverlapSphere(center, range, layer)
            .Where(x =>
            {
                Vector3 resultPos = x.transform.position - center;
                resultPos.y = 0;
                float toCompare = Vector3.Angle(resultPos, forward);
                return toCompare >= -(angle / 2f) && toCompare <= angle / 2f;
            })
            .ToArray();

        return result;
    }



    static List<Collider> GetCollidersFromCast(RaycastHit[] hits, Transform initialCollider, string targetTag, string tagToIgnore)
    {
        bool hasHitOtherThanTarget = false;
        List<Collider> enemies = new List<Collider>();
        List<Vector3> obstaclesHitPos = new List<Vector3>();

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.gameObject.CompareTag(targetTag))
            {
                if (tagToIgnore != string.Empty && !hit.collider.gameObject.CompareTag(tagToIgnore))
                {
                    //used to have the position in world space when colliding with a meshCollider(MESH COLLIDER NEEDS TO BE ACTIVATED AS CONVEX TO WORK)
                    //Debug.Log(hit.collider.transform.TransformPoint(hit.point));
                    obstaclesHitPos.Add(hit.collider.transform.TransformPoint(hit.point));
                    hasHitOtherThanTarget = true;
                }
                else if (tagToIgnore == string.Empty)
                {
                    //Debug.Log("hit raté : " + hit.collider.gameObject.name);
                    hasHitOtherThanTarget = true;
                }
            }
            else
            {
                enemies.Add(hit.collider);
            }
        }

        if (!hasHitOtherThanTarget)
        {
            return enemies;
        }
        else
        {
            List<Collider> enemiesAheadOfObstacles = new List<Collider>();
            foreach (Collider enemy in enemies)
            {
                foreach (Vector3 obstacleHitPos in obstaclesHitPos)
                {
                    float colliderToObstacleDist = (obstacleHitPos - initialCollider.transform.position).magnitude;
                    float colliderToenemyDist = (enemy.transform.position - initialCollider.transform.position).magnitude;

                    if (colliderToObstacleDist >= colliderToenemyDist && !enemiesAheadOfObstacles.Contains(enemy))
                    {
                        enemiesAheadOfObstacles.Add(enemy);
                    }
                }
            }

            return enemiesAheadOfObstacles;
        }
    }
}
