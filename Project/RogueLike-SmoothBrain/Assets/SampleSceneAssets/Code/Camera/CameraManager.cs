using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera deahtCam;
    private Hero player;

    void Awake()
    {
        mainCam.m_Priority = 1;
        deahtCam.m_Priority = -1;
    }

    private void SwitchCam(Vector3 _)
    {
        mainCam.m_Priority = -1;
        deahtCam.m_Priority = 1;
    }

    private void OnEnable()
    {
        Utilities.Hero.OnDeath += SwitchCam;
    }

    private void OnDisable()
    {
        Utilities.Hero.OnDeath -= SwitchCam;
    }
}
