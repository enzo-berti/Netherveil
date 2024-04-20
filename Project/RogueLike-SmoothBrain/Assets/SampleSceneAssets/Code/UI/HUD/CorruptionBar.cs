using UnityEngine;
using UnityEngine.UI;

public class CorruptionBar : MonoBehaviour
{
    [SerializeField] private Slider positive;
    [SerializeField] private Slider negative;
    [SerializeField] private Vector2 EdgeValue;

    void Start()
    {
        positive.maxValue = 50;
        negative.maxValue = 50;
        positive.minValue = 0;
        negative.minValue = 0;
        positive.value = positive.minValue;
        negative.value = negative.maxValue;
    }

    void Update()
    {
        negative.value = positive.value;
    }
}
