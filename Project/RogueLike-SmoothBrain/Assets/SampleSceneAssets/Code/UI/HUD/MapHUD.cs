using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class MapHUD : MonoBehaviour
{
    private GameObject minimapCam;
    private GameObject bigmapCam;
    [SerializeField] private GameObject hud;
    [SerializeField] private RectTransform miniMap;
    [SerializeField] private RectTransform bigMap;
    private bool isMiniMapActive = true;
    private Coroutine miniMapRoutine;
    private Coroutine bigMapRoutine;

    private void Start()
    {
        minimapCam = FindObjectOfType<MiniMapCam>(true).gameObject;
        bigmapCam = FindObjectOfType<BigMapCam>(true).gameObject;
    }

    private void OnDisable()
    {
        if (miniMapRoutine != null)
        {
            StopCoroutine(miniMapRoutine);
            miniMap.localScale = Vector3.one * 0.1f;
        }
        if (bigMapRoutine != null)
        {
            StopCoroutine(bigMapRoutine);
            bigMap.localScale = Vector3.zero;
        }
    }

    public void Toggle()
    {
        if (miniMapRoutine != null)
            StopCoroutine(miniMapRoutine);
        if (bigMapRoutine != null)
            StopCoroutine(bigMapRoutine);

        if (isMiniMapActive)
        {
            minimapCam.SetActive(false);
            bigmapCam.SetActive(true);

            miniMapRoutine = StartCoroutine(miniMap.DownScaleCoroutine(0.1f));
            bigMapRoutine = StartCoroutine(bigMap.UpScaleCoroutine(0.1f));
        }
        else
        {
            minimapCam.SetActive(true);
            bigmapCam.SetActive(false);

            miniMapRoutine = StartCoroutine(miniMap.UpScaleCoroutine(0.1f));
            bigMapRoutine = StartCoroutine(bigMap.DownScaleCoroutine(0.1f));
        }

        isMiniMapActive = !isMiniMapActive;
    }
}
