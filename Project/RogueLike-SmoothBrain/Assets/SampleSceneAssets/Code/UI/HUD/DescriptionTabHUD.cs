using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public class DescriptionTabHUD : MonoBehaviour
{
    [SerializeField] private RectTransform tabRectTransform;
    [SerializeField] private TMP_Text titleTextMesh;
    [SerializeField] private TMP_Text descriptionTextMesh;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Image background;
    [SerializeField] private Button CloseButton;

    public void SetTab(string title, string description, VideoClip clip, Sprite background)
    {
        titleTextMesh.text = title;
        descriptionTextMesh.text = description;
        videoPlayer.clip = clip;
        this.background.sprite = background;
    }

    public void OpenTab()
    {
        StartCoroutine(tabRectTransform.UpScaleCoroutine(0.1f));
        EventSystem.current.SetSelectedGameObject(CloseButton.gameObject);
    }

    public void CloseTab()
    {
        StartCoroutine(tabRectTransform.DownScaleCoroutine(0.1f));
    }
}
