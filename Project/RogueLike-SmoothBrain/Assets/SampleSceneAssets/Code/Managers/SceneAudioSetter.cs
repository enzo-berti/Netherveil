using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomEventTrigger : EventTrigger
{
    public static EventReference buttonSelectSFX;
    public override void OnSelect(BaseEventData data)
    {
        AudioManager.Instance.PlaySound(buttonSelectSFX);
    }
}

public class SceneAudioSetter : MonoBehaviour
{
    [SerializeField] EventReference buttonClick;
    [SerializeField] EventReference buttonSelect;
    void Start()
    {
        CustomEventTrigger.buttonSelectSFX = buttonSelect;
        Button[] buttons = FindObjectsOfType<Button>(true); // parameter makes it include inactive UI elements with buttons
        foreach (Button b in buttons)
        {
            b.onClick.AddListener(ButtonClickSFX);
            b.AddComponent<CustomEventTrigger>();
        }
    }
    public void ButtonClickSFX()
    {
        AudioManager.Instance.PlaySound(buttonClick);
    }
}
