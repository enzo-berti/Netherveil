using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private float offset = 0.4f;
    [SerializeField] private float randomOffset = 0.03f;
    [SerializeField] private float timeFactor = 2.25f;
    [SerializeField] private new Light light;
    private float startIntensity;

    private void Start()
    {
        startIntensity = light.intensity;
    }

    private void Update()
    {
        light.intensity = startIntensity + Mathf.Sin(Time.time * timeFactor) * offset + Random.Range(-randomOffset, randomOffset);
    }
}
