using UnityEngine;
using UnityEngine.InputSystem;

public class HudHandler : MonoBehaviour
{
    [SerializeField] private GameObject miniMap;
    [SerializeField] private GameObject bigMap;

    public void ToggleMap(InputAction.CallbackContext ctx)
    {
        if (miniMap.activeSelf)
        {
            miniMap.SetActive(false);
            bigMap.SetActive(true);
        }
        else
        {
            miniMap.SetActive(true);
            bigMap.SetActive(false);
        }
    }
}
