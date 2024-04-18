using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLifeBar : MonoBehaviour
{
    [Header("Gameobjects & Components")]
    [SerializeField] private Image lifeBarSlider;
    [SerializeField] private Image damageBarSlider;
    private RectTransform barRect;

    private float maxValue;
    private float value;

    [Header("Parameters")]
    [SerializeField, MinMaxSlider(20.0f, 500.0f)] private Vector2 barSizeClamp = new Vector2(100.0f, 300.0f);
    [SerializeField, Range(0.1f, 1.0f)] private float damageDisplayTime = 0.3f;
    private Coroutine damageRoutine = null;

    // getter and setters
    public float MaxValue => maxValue;
    public float Value => value;
    private float FactorValue => value / maxValue;

    private void Awake()
    {
        barRect = GetComponent<RectTransform>();
    }

    public void SetMaxValue(float value)
    {
        maxValue = value;

        // update size life bar
        ResizeLifeBar();
    }
    
    public void ValueChanged(float value)
    {
        // update life bar
        this.value = value;
        lifeBarSlider.fillAmount = FactorValue;

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
        float barDiff = damageBarSlider.fillAmount - lifeBarSlider.fillAmount;

        while (lifeBarSlider.fillAmount < damageBarSlider.fillAmount)
        {
            damageBarSlider.fillAmount -= Time.deltaTime * barDiff;
            yield return null;
        }

        damageRoutine = null;
    }
}
