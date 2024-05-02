using TMPro;
using Unity.VisualScripting;
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

        private void Start()
        {
            fountain = GetComponent<Fountain>();

            rectTransform.localScale = Vector3.zero;
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
            string type = fountain.Type == FountainType.Blessing ? "<color=yellow>benediction</color>" : "<color=purple>corruption</color>";
            string blood = $"<color=red>{fountain.BloodPrice} blood{(fountain.BloodPrice > 1 ? "s" : string.Empty)}</color>";
            string trade = $"<color=yellow>{fountain.AbsoluteValueTrade}</color>";

            displayTextMesh.text = $"Use {blood} to gain {trade} of {type}.";
        }
    }
}
