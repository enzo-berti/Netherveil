using System.Linq;
using UnityEditor;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] private float range = 10f;
    [SerializeField, Range(0f, 360f)] private float angle = 70f;

    public Transform GetTarget()
    {
        Transform target = Physics.OverlapSphere(transform.position, angle)
            .Where(x => x.CompareTag("Player"))
            .Select(x => x.transform)
            .FirstOrDefault();

        if (target == null)
            return null;

        if (IsBetweenAngle(Vector3.Angle(target.position - transform.position, transform.forward)))
        {
            return target;
        }
        return null;
    }

    private bool IsBetweenAngle(float toCompare)
    {
        return -angle / 2f <= toCompare && angle / 2f >= toCompare;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //if (Selection.activeGameObject != gameObject)
        //    return;

        Handles.color = new Color(1, 0, 0, 0.25f);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle / 2f, range);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle / 2f, range);
        Handles.DrawSolidDisc(transform.position, Vector3.up, 2);
    }
#endif
}