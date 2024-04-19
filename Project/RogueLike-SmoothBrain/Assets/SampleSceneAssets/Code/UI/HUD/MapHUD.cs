using System.Linq;
using UnityEngine;

public class MapHUD : MonoBehaviour
{
    private GameObject minimapCam;
    private GameObject bigmapCam;
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject bigMap;

    private void Start()
    {
        MiniMapCam[] miniMapCams = FindObjectsOfType<MiniMapCam>(true).OrderBy(x => x.GetComponent<Camera>().orthographicSize).ToArray();
        minimapCam = miniMapCams[0].gameObject;
        bigmapCam = miniMapCams[1].gameObject;
    }

    public void Toggle()
    {
        if (miniMap.activeSelf)
        {
            minimapCam.SetActive(false);
            miniMap.SetActive(false);

            bigmapCam.SetActive(true);
            bigMap.SetActive(true);
        }
        else
        {
            minimapCam.SetActive(true);
            miniMap.SetActive(true);

            bigmapCam.SetActive(false);
            bigMap.SetActive(false);
        }
    }
}
