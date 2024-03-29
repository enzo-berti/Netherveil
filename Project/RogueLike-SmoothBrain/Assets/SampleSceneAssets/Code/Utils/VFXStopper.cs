using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class VFXStopper : MonoBehaviour
{
    [SerializeField] VisualEffect effect;
    float duration;


    private void Start()
    {
        duration = effect.GetFloat("Duration");
    }

    public void PlayVFX()
    {
        effect.Play();
        StartCoroutine(StopVFXCoroutine());
    }

    IEnumerator StopVFXCoroutine()
    {
        yield return new WaitForSeconds(duration);
        effect.Stop();
    }
}
