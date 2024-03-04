using UnityEngine;

public class PressurePlateTrap : MonoBehaviour
{
    public GameObject[] trapToActivate;
    public float cooldownTime = 2f;
    private float lastActivationTime;
    private bool firstActivation = true;

    private void OnTriggerEnter(Collider other)
    {
        if (firstActivation || Time.time - lastActivationTime >= cooldownTime && other.CompareTag("Player"))
        {
            ActivateTraps();
            if (firstActivation)
            {
                firstActivation = false;
            }
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
        lastActivationTime = Time.time;
    }

    private void Update()
    {
        if (!firstActivation && Time.time - lastActivationTime < cooldownTime)
        {
            cooldownTime -= Time.deltaTime;
            if (cooldownTime < 0f)
            {
                cooldownTime = 0f;
            }
        }
    }
}