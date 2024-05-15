using UnityEngine;

public class PressurePlateTrap : MonoBehaviour
{
    [SerializeField] ParticleSystem vfx;
    public GameObject[] trapToActivate;
    public float cooldownTime = 2f;
    private float currentCooldownTime = 0f;
    private bool canActive = true;
    public Sound activeSound;

    private void OnTriggerStay(Collider other)
    {
        if (canActive && other.TryGetComponent(out IDamageable damageable) && (damageable as MonoBehaviour).TryGetComponent(out Entity entity) && entity.canTriggerTraps)
        {
            ActivateTraps();
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

        vfx.Play();
        activeSound.Play(transform.position);
        canActive = false;
    }

    private void Update()
    {
        if (!canActive)
        {
            currentCooldownTime += Time.deltaTime;
            if (currentCooldownTime >= cooldownTime)
            {
                currentCooldownTime = 0;
                canActive = true;
            }
        }
    }
}