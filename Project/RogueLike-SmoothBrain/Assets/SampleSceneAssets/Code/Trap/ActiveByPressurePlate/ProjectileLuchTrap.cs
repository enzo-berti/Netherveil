using FMODUnity;
using UnityEngine;

public class ProjectileLuchTrap : MonoBehaviour , IActivableTrap
{
    [SerializeField] GameObject itemToInstanciate;
    [SerializeField] EventReference throwProjectilSFX;
    private FMOD.Studio.EventInstance throwProjectilEvent;
    Vector3 launchPos;

    private void Awake()
    {
        throwProjectilEvent = RuntimeManager.CreateInstance(throwProjectilSFX);
    }

    private void Start()
    {
        launchPos = GetComponentInChildren<Transform>().position;
    }

    public void Active()
    {
        AudioManager.Instance.StopSound(throwProjectilEvent, FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioManager.Instance.PlaySound(throwProjectilSFX);
        Instantiate(itemToInstanciate, launchPos + GetComponentInChildren<Transform>().forward * 3, GetComponentInChildren<Transform>().rotation);
    }
}
