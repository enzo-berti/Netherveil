using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private float offset = 0.4f;
    [SerializeField] private float randomOffset = 0.03f;
    [SerializeField] private float timeFactor = 2.25f;
    [SerializeField] private Light lightobject;
    private float startIntensity;

    private void Start()
    {
        startIntensity = lightobject.intensity;
    }

    private void Update()
    {
        lightobject.intensity = startIntensity + Mathf.Sin(Time.time * timeFactor) * offset + Random.Range(-randomOffset, randomOffset);
    }
}
