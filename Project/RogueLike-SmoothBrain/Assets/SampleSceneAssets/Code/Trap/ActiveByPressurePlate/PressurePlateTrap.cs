using UnityEngine;

public class PressurePlateTrap : MonoBehaviour
{
    [SerializeField] ParticleSystem vfx;
    [SerializeField] private GameObject[] trapToActivate;
    [SerializeField] private Sound activeSound;
    [SerializeField] private GameObject plateToMove;

    private bool isPressed = false;
    private Vector3 intialePos;

    private void Awake()
    {
        intialePos = plateToMove.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPressed && other.TryGetComponent(out IDamageable damageable) && (damageable as MonoBehaviour).TryGetComponent(out Entity entity) && entity.canTriggerTraps)
        {
            ActivateTraps();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPressed && other.TryGetComponent(out IDamageable damageable) && (damageable as MonoBehaviour).TryGetComponent(out Entity entity) && entity.canTriggerTraps)
        {
            isPressed = false;
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
        isPressed = true;
    }
}