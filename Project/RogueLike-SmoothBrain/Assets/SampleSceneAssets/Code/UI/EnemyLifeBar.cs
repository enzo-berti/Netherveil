using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLifeBar : MonoBehaviour
{
    [Header("Gameobjects & Components")]
    [SerializeField] private Slider lifeBarSlider;
    [SerializeField] private Slider damageBarSlider;
    private RectTransform barRect;

    [Header("Parameters")]
    [SerializeField, MinMaxSlider(20.0f, 500.0f)] private Vector2 barSizeClamp = new Vector2(100.0f, 300.0f);
    [SerializeField, Range(0.1f, 1.0f)] private float damageDisplayTime = 0.3f;
    private Coroutine damageRoutine = null;

    // getter and setters
    public float maxValue => lifeBarSlider.maxValue;
    public float value => lifeBarSlider.value;

    private void Start()
    {
        barRect = GetComponent<RectTransform>();
    }

    public void SetMaxValue(float value)
    {
        // set max value in sliders
        lifeBarSlider.maxValue = value;
        damageBarSlider.maxValue = value;

        // set value by the max
        lifeBarSlider.value = maxValue;
        damageBarSlider.value = maxValue;

        // update size life bar
        ResizeLifeBar();
    }
    
    public void ValueChanged(float value)
    {
        // update life bar
        lifeBarSlider.value = value;

        // stop damage update if is running
        if (damageRoutine != null)
            StopCoroutine(damageRoutine);

        // start damage update
        damageRoutine = StartCoroutine(DamageBarCoroutine());
    }

    private void ResizeLifeBar()
    {
        // get size
        Vector2 size = barRect.sizeDelta;

        size.x *= maxValue / barSizeClamp.x;
        size.x = Mathf.Clamp(size.x, barSizeClamp.x, barSizeClamp.y);

        // set size
        barRect.sizeDelta = size;
    }

    private IEnumerator DamageBarCoroutine()
    {
        yield return new WaitForSeconds(damageDisplayTime);
        float barDiff = damageBarSlider.value - lifeBarSlider.value;

        while (lifeBarSlider.value < damageBarSlider.value)
        {
            damageBarSlider.value -= Time.deltaTime * barDiff;
            yield return null;
        }

        damageRoutine = null;
    }
}
