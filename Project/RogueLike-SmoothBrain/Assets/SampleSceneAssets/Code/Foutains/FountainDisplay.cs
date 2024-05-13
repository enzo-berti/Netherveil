using TMPro;
using UnityEngine;

namespace Fountain
{
    [RequireComponent(typeof(Fountain))]
    public class FountainDisplay : MonoBehaviour
    {
        private Fountain fountain;

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private TMP_Text displayTextMesh;
        private Coroutine displayRoutine;
        private float displayDuration = 0.2f;
        private float originalSize;
        private float iconSize;

        private void Start()
        {
            fountain = GetComponent<Fountain>();

            originalSize = displayTextMesh.fontSize;
            iconSize = originalSize + 10;
            rectTransform.localScale = Vector3.zero;

            Utilities.Hero.OnBenedictionMaxUpgrade += ReloadDisplay;
            Utilities.Hero.OnCorruptionMaxUpgrade += ReloadDisplay;
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
            SetText(fountain);

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

        private void SetText(Fountain fountain)
        {
            string blood = $"{fountain.BloodPrice}<size={iconSize}><sprite name=\"blood\"><size={originalSize}>";

            string value = fountain.Type == FountainType.Blessing ? $"<color=yellow>{fountain.AbsoluteValueTrade}</color>" : 
                $"<color=purple>{fountain.AbsoluteValueTrade}</color>";

            string trade = value + " " + 
                (fountain.Type == FountainType.Blessing ? $"<size={iconSize}><sprite name=\"benediction\"><size={originalSize}>" :
                $"<size={iconSize}><sprite name=\"corruption\"><size={originalSize}>");

            if(fountain.GotMaxInAnyAlignment())
            {
                displayTextMesh.text = "Max " + 
                    (fountain.Type == FountainType.Blessing ? 
                    $"<size={iconSize}><sprite name=\"benediction\"><size={originalSize}>" :
                    $"<size={iconSize}><sprite name=\"corruption\"><size={originalSize}>") + " Obtained";
            }
            else
            {
                displayTextMesh.text = $"Use {blood} to gain {trade}.";
            }
        }

        private void ReloadDisplay(ISpecialAbility ability)
        {
            SetText(fountain);
        }
    }
}
