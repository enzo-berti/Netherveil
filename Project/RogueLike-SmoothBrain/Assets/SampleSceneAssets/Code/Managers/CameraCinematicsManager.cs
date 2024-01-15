using Cinemachine;
using System.Linq;
using UnityEngine;

public class CameraCinematicsManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] cameras;

    private void Start()
    {
        if (cameras.Any())
            ActiveCamera(cameras.First());
    }
        
    public void ActiveCamera(CinemachineVirtualCamera toActive) 
    {
        foreach (var cam in cameras)
        {
            cam.Priority = 0;
        }

        toActive.Priority = 1;
    }
}
