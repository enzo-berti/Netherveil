using UnityEngine;

public static class GizmosUtilities
{
    public static void DrawWireCapsule(Vector3 p1, Vector3 p2, float radius)
    {
#if UNITY_EDITOR
        // Special case when both points are in the same position
        if (p1 == p2)
        {
            Gizmos.DrawWireSphere(p1, radius);
            return;
        }

        Quaternion p1Rotation = Quaternion.LookRotation(p1 - p2);
        Quaternion p2Rotation = Quaternion.LookRotation(p2 - p1);
        // Check if capsule direction is collinear to Vector.up
        float c = Vector3.Dot((p1 - p2).normalized, Vector3.up);
        if (c == 1f || c == -1f)
        {
            // Fix rotation
            p2Rotation = Quaternion.Euler(p2Rotation.eulerAngles.x, p2Rotation.eulerAngles.y + 180f, p2Rotation.eulerAngles.z);
        }
        using (new UnityEditor.Handles.DrawingScope(Gizmos.color, Gizmos.matrix))
        {
            // First side
            UnityEditor.Handles.DrawWireArc(p1, p1Rotation * Vector3.left, p1Rotation * Vector3.down, 180f, radius);
            UnityEditor.Handles.DrawWireArc(p1, p1Rotation * Vector3.up, p1Rotation * Vector3.left, 180f, radius);
            UnityEditor.Handles.DrawWireDisc(p1, (p2 - p1).normalized, radius);
            // Second side
            UnityEditor.Handles.DrawWireArc(p2, p2Rotation * Vector3.left, p2Rotation * Vector3.down, 180f, radius);
            UnityEditor.Handles.DrawWireArc(p2, p2Rotation * Vector3.up, p2Rotation * Vector3.left, 180f, radius);
            UnityEditor.Handles.DrawWireDisc(p2, (p1 - p2).normalized, radius);
        }
        // Lines
        Gizmos.DrawLine(p1 + p1Rotation * Vector3.down * radius, p2 + p2Rotation * Vector3.down * radius);
        Gizmos.DrawLine(p1 + p1Rotation * Vector3.left * radius, p2 + p2Rotation * Vector3.right * radius);
        Gizmos.DrawLine(p1 + p1Rotation * Vector3.up * radius, p2 + p2Rotation * Vector3.up * radius);
        Gizmos.DrawLine(p1 + p1Rotation * Vector3.right * radius, p2 + p2Rotation * Vector3.left * radius);
#endif
    }
}


public class AttackCollision : MonoBehaviour
{
    public enum CollisionType
    {
        Ray,
        Box,
        Capsule,
        Sphere
    }

#if UNITY_EDITOR // only used for gizmo
    [SerializeField] private Color collideColor = Color.white;
#endif
    [SerializeField] private CollisionType collisionType;
    // Cube
    [SerializeField] private Vector3 size;
    // Ray
    [SerializeField] private Vector3 mouvement;
    // Sphere & Capsule
    [SerializeField] private float radius;
    // Capsule
    [SerializeField] private float height;
    [SerializeField, Range(0, 2)] private int direction;

    private void OnDrawGizmos()
    {
        Gizmos.color = collideColor;

        switch (collisionType)
        {
            case CollisionType.Ray:
                Vector3 to = transform.position;
                to += mouvement;
                Gizmos.DrawLine(transform.position, to);
                break;
            case CollisionType.Box:
                Gizmos.DrawWireCube(transform.position, new Vector3(size.x * 2, size.y * 2, size.z * 2));
                break;
            case CollisionType.Capsule:
                Vector3 offset = Vector3.zero; offset[direction] = height;
                GizmosUtilities.DrawWireCapsule(transform.position + offset, transform.position - offset, radius);
                break;
            case CollisionType.Sphere:
                Gizmos.DrawWireSphere(transform.position, radius);
                break;
            default:
                break;
        }
    }

    private void OnValidate()
    {
        Initialize();
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        
    }

    public void Launch()
    {

    }
}
