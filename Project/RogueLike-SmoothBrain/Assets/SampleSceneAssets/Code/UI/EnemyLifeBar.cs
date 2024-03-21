using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLifeBar : MonoBehaviour
{
    [SerializeField] Slider lifeBarSlider;
    [SerializeField] Slider damageBarSlider;
    bool coroutineRunning = false;
    
    public void ValueChanged(float value)
    {
        if(coroutineRunning)
        {
            StopAllCoroutines();
            coroutineRunning = false;
        }
        StartCoroutine(DamageBarCoroutine());
    }

    private IEnumerator DamageBarCoroutine()
    {
        coroutineRunning = true;
        yield return new WaitForSeconds(0.3f);
        float barDiff = damageBarSlider.value - lifeBarSlider.value;
        while (lifeBarSlider.value < damageBarSlider.value)
        {
            damageBarSlider.value -= Time.deltaTime * barDiff;
            yield return null;
        }

        coroutineRunning = false;
        yield return null;
    }
}
