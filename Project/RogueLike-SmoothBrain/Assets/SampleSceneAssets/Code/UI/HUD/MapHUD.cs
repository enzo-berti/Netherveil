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

    private void Start()
    {
        minimapCam = FindObjectOfType<MiniMapCam>(true).gameObject;
        bigmapCam = FindObjectOfType<BigMapCam>(true).gameObject;
    }

    public void Toggle()
    {
        if (isMiniMapActive)
        {
            minimapCam.SetActive(false);
            bigmapCam.SetActive(true);

            StartCoroutine(miniMap.DownScaleCoroutine(0.1f));
            StartCoroutine(bigMap.UpScaleCoroutine(0.1f));
        }
        else
        {
            minimapCam.SetActive(true);
            bigmapCam.SetActive(false);

            StartCoroutine(miniMap.UpScaleCoroutine(0.1f));
            StartCoroutine(bigMap.DownScaleCoroutine(0.1f));
        }

        isMiniMapActive = !isMiniMapActive;
    }
}
