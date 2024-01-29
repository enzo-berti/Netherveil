using System.Linq;
using UnityEditor;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] bool renderVisionCone = true;
    [SerializeField] private float range = 10f;
    [SerializeField, Range(0f, 360f)] public float angle = 70f;
    [HideInInspector] public bool extendedVisionCone = false;
    [SerializeField] float extentionCoeff = 1.5f;

    // chions dans le code
    [SerializeField] float extendedHitboxSize;
    private float OGExtendedHitboxSize;

    private void Start()
    {
        OGExtendedHitboxSize = extendedHitboxSize;
        extendedHitboxSize = 0;
    }

    public Transform GetTarget(string tag)
    {
        Transform target = Physics.OverlapSphere(transform.position, range)
            .Where(x => x.CompareTag(tag))
            .Select(x => x.transform)
            .FirstOrDefault();

        if (target == null)
            return null;

        if ((transform.position - target.position).magnitude < extendedHitboxSize)
            return target;

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

    public void ToggleExtendedVisionCone(bool _state)
    {
        if (extendedVisionCone != _state)
        {
            extendedVisionCone = _state;

            if (extendedVisionCone)
            {
                range *= extentionCoeff;
                angle *= extentionCoeff;
                extendedHitboxSize = OGExtendedHitboxSize;
            }
            else
            {
                range /= extentionCoeff;
                angle /= extentionCoeff;
                extendedHitboxSize = 0;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //if (Selection.activeGameObject != gameObject)
        //    return;
        if(renderVisionCone)
        {
            Handles.color = new Color(1, 0, 0, 0.25f);
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle / 2f, range);
            Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle / 2f, range);
            Handles.DrawSolidDisc(transform.position, Vector3.up, extendedHitboxSize);
        }
    }
#endif
}