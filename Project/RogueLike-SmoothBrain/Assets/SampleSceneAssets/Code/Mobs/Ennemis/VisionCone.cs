using System.Linq;
using UnityEditor;
using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [SerializeField] float VisionRange;
    [SerializeField, Range(0f, 360f)] float VisionAngle;
    [SerializeField] Transform player;
    public Transform target;

    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, VisionAngle / 2f, VisionRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -VisionAngle / 2f, VisionRange);
    }

    public bool SeesPlayer()
    {
        Transform player = Physics.OverlapSphere(transform.position, VisionAngle)
            .Where(x => x.CompareTag("Player"))
            .Select(x => x.transform)
            .FirstOrDefault();

        if (player == null)
            return false;

        target = player;

        float enemyToPlayerAngle = Vector3.Angle(player.position - transform.position, transform.forward);

        return -VisionAngle / 2f <= enemyToPlayerAngle && VisionAngle / 2f >= enemyToPlayerAngle;
    }
}