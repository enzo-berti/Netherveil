using UnityEngine;

public class PressurePlateTrap : MonoBehaviour
{
    public GameObject[] trapToActivate;
    public float cooldownTime = 2f;
    private float currentCooldownTime = 0;
    private bool canActive = true;


    private void OnTriggerStay(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (canActive && damageable != null)
        {
            ActivateTraps();
        }
    }

    private void ActivateTraps()
    {
        for (int i = 0; i < trapToActivate.Length; i++)
        {
            IActivableTrap activableTrap = trapToActivate[i].GetComponent<IActivableTrap>();
            if (activableTrap != null)
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