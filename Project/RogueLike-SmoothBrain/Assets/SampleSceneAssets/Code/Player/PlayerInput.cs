using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    PlayerInputMap playerInputMap;
    PlayerController controller;
    PlayerAnimation m_animation;
    // Start is called before the first frame update
    void Awake()
    {
        playerInputMap = new PlayerInputMap();
        controller = GetComponent<PlayerController>();
        m_animation = GetComponent<PlayerAnimation>();
    }

    private void OnEnable()
    {
        playerInputMap.Enable();
        playerInputMap.Movement.Movement.performed += controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled += controller.ReadDirection;
        playerInputMap.Attack.Attack.performed += m_animation.Attack;
        playerInputMap.Dash.Dash.performed += m_animation.Dash;
    }

    private void OnDisable()
    {
        playerInputMap.Disable();
        playerInputMap.Movement.Movement.performed -= controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled -= controller.ReadDirection;
        playerInputMap.Attack.Attack.performed -= m_animation.Attack;
        playerInputMap.Dash.Dash.performed -= m_animation.Dash;
    }
}
