using UnityEngine;

public class PressurePlateTrap : MonoBehaviour
{
    public GameObject[] trapToActivate;
    public float cooldownTime = 2f;
    private float currentCooldownTime = 0f;
    private bool canActive = true;

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