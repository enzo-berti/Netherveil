using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FinalBossCloneBehaviour : MonoBehaviour
{
    VisualEffect VFXBomb;

    private void Awake()
    {
        VFXBomb = GetComponentInChildren<VisualEffect>();
    }

    public void Explode()
    {
        VFXBomb.Play();
    }

    IEnumerator ExplosionCoroutine()
    {
        float timer = 0f;
        float timeToExplode = VFXBomb.GetFloat("ExplosionTime");

        while (timer < timeToExplode)
        {
            timer += Time.deltaTime;
            yield return null;
        }



        yield return null;
    }
}
