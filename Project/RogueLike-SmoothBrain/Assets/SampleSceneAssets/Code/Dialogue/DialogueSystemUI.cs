using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystemUI : MonoBehaviour
{
    public enum DialogueMode
    {
        Illustration,
        NoIllustration,
        Message
    }

    [Header("GameObjects & Components")]
    [SerializeField] private TMP_Text nameMesh;
    [SerializeField] private TMP_Text dialogueMesh;
    [SerializeField] private Image illustrationImage;

    [Space, Header("Parameters")]
    [SerializeField] private DialogueMode mode;
    public DialogueMode Mode
    {
        get => mode;
        set
        {
            mode = value;
            UpdateMode();
        }
    }

    private void UpdateMode()
    {
        switch (mode)
        {
            case DialogueMode.Illustration:
                nameMesh.gameObject.SetActive(true);
                illustrationImage.gameObject.SetActive(true);
                dialogueMesh.alignment = TextAlignmentOptions.TopLeft;
                break;
            case DialogueMode.NoIllustration:
                nameMesh.gameObject.SetActive(true);
                illustrationImage.gameObject.SetActive(false);
                dialogueMesh.alignment = TextAlignmentOptions.TopLeft;
                break;
            case DialogueMode.Message:
                nameMesh.gameObject.SetActive(false);
                illustrationImage.gameObject.SetActive(false);
                dialogueMesh.alignment = TextAlignmentOptions.Center;
                break;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateMode();
    }
#endif
}
