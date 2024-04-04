using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class VFXStopper : MonoBehaviour
{
    [SerializeField] VisualEffect effect;
    public float Duration { get; set; } = 0f;
    [SerializeField] bool destroyOnStop = false;


    private void Start()
    {
        if(effect.HasFloat("Duration"))
        {
            Duration = effect.GetFloat("Duration");
        }
    }

    public void PlayVFX()
    {
        if(gameObject.activeInHierarchy)
        {
            effect.Play();
            StartCoroutine(StopVFXCoroutine());
        }
    }

    IEnumerator StopVFXCoroutine()
    {
        yield return new WaitForSeconds(Duration);
        effect.Stop();
        if(destroyOnStop)
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
