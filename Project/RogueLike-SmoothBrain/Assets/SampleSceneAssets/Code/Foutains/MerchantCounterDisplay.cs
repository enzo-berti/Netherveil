using TMPro;
using UnityEngine;

public class MerchantCounterDisplay : MonoBehaviour
{
    private MerchantCounter merchantCounter;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TMP_Text displayTextMesh;
    private Coroutine displayRoutine;
    private float displayDuration = 0.2f;
    private float originalSize;
    private float iconSize;

    private void Start()
    {
        merchantCounter = GetComponent<MerchantCounter>();

        originalSize = displayTextMesh.fontSize;
        iconSize = originalSize + 10;
        rectTransform.localScale = Vector3.zero;
    }

    private void OnDisable()
    {
        if (displayRoutine != null)
        {
            StopCoroutine(displayRoutine);
            rectTransform.localScale = Vector3.zero;
        }
    }

    public void Display()
    {
        SetText(merchantCounter);

        if (displayRoutine != null)
            StopCoroutine(displayRoutine);

        displayRoutine = StartCoroutine(rectTransform.UpScaleCoroutine(displayDuration, 0.01f));
    }

    public void Undisplay()
    {
        if (displayRoutine != null)
            StopCoroutine(displayRoutine);
        displayRoutine = StartCoroutine(rectTransform.DownScaleCoroutine(displayDuration, 0.01f));
    }

    private void SetText(MerchantCounter _merchantCounter)
    {
        string blood = $"{_merchantCounter.BloodPrice}<size={iconSize}><sprite name=\"blood\"><size={originalSize}>";

        displayTextMesh.text = $"Use {blood} to gain {_merchantCounter.ValueTrade} HP.";
    }
}
