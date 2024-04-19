using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Fountain
{
    [RequireComponent(typeof(Fountain))]
    public class FountainDisplay : MonoBehaviour
    {
        private Fountain fountain;

        [SerializeField] private Canvas canvas;
        [SerializeField] private TMP_Text displayTextMesh;
        private Coroutine displayRoutine;
        private float displayDuration = 0.2f;

        private void Start()
        {
            fountain = GetComponent<Fountain>();

            canvas.transform.localScale = Vector3.zero;
        }

        public void Display()
        {
            SetText(fountain);

            if (displayRoutine != null)
                StopCoroutine(displayRoutine);
            displayRoutine = StartCoroutine(UITween.UpScaleCoroutine(canvas.transform, displayDuration, 0.01f));
        }

        public void Undisplay()
        {
            if (displayRoutine != null)
                StopCoroutine(displayRoutine);
            displayRoutine = StartCoroutine(UITween.DownScaleCoroutine(canvas.transform, displayDuration, 0.01f));
        }

        private void SetText(Fountain fountain)
        {
            string type = fountain.Type == FountainType.Blessing ? "<color=#00F0FF>blessing</color>" : "<color=#9E57C1>corruption</color>";
            string blood = $"<color=red>{fountain.BloodPrice} blood{(fountain.BloodPrice > 1 ? "s" : string.Empty)}</color>";
            string trade = $"<color=yellow>{fountain.AbsoluteValueTrade}</color>";

            displayTextMesh.text = $"Use {blood} to gain {trade} of {type}.";
        }
    }
}
