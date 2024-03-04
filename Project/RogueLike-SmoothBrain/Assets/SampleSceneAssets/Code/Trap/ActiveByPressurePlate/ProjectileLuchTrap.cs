using UnityEngine;

public class ProjectileLuchTrap : MonoBehaviour , IActivableTrap
{
    [SerializeField] GameObject itemToInstanciate;
    Vector3 launchPos;

    private void Start()
    {
        launchPos = GetComponentInChildren<Transform>().position;
    }

    public void Active()
    {
        Instantiate(itemToInstanciate, launchPos, transform.rotation);
    }
}
