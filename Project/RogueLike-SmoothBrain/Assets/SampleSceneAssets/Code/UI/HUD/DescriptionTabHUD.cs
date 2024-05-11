using System.Collections;
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
    float duration = 0.1f;

    public void SetTab(string title, string description, VideoClip clip, Sprite background)
    {
        titleTextMesh.text = title;
        descriptionTextMesh.text = description;
        videoPlayer.clip = clip;
        this.background.sprite = background;
    }

    public void OpenTab()
    {
        StartCoroutine(tabRectTransform.UpScaleCoroutine(duration));
        StartCoroutine(PauseGame());
        EventSystem.current.SetSelectedGameObject(CloseButton.gameObject);
    }

    public void CloseTab()
    {
        Time.timeScale = 1;
        StartCoroutine(tabRectTransform.DownScaleCoroutine(duration));
    }

    private IEnumerator PauseGame()
    {
        yield return new WaitForSeconds(duration);
        Time.timeScale = 0;
    }
}
