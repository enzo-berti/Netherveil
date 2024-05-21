using UnityEngine;

public class PressurePlateTrap : MonoBehaviour
{
    [SerializeField] ParticleSystem vfx;
    public GameObject[] trapToActivate;
    private bool canActive = true;
    public Sound activeSound;
    public GameObject plateToMove;
    private Vector3 intialePos;

    private void Start()
    {
        intialePos = plateToMove.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canActive && other.TryGetComponent(out IDamageable damageable) && (damageable as MonoBehaviour).TryGetComponent(out Entity entity) && entity.canTriggerTraps)
        {
            ActivateTraps();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!canActive && other.TryGetComponent(out IDamageable damageable) && (damageable as MonoBehaviour).TryGetComponent(out Entity entity) && entity.canTriggerTraps)
        {
            canActive = true;
            plateToMove.transform.position = intialePos;
        }
    }

    private void ActivateTraps()
    {
        foreach (var t in trapToActivate)
        {
            if (t.TryGetComponent(out IActivableTrap activableTrap))
            {
                activableTrap.Active();
            }
        }

        plateToMove.transform.position = new Vector3(intialePos.x, intialePos.y - .1f, intialePos.z);
        vfx.Play();
        activeSound.Play(transform.position);
        canActive = false;
    }
}