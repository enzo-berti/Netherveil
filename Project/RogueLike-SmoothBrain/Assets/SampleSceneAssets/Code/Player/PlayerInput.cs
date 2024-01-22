using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class PlayerInput : MonoBehaviour
{
    PlayerInputMap playerInputMap;
    PlayerController controller;
    PlayerAnimation m_animation;
    PlayerInteractions m_interaction;
    PlayerAttack m_attack;
    // Start is called before the first frame update
    void Awake()
    {
        playerInputMap = new PlayerInputMap();
        controller = GetComponent<PlayerController>();
        m_animation = GetComponent<PlayerAnimation>();
        m_interaction = GetComponent<PlayerInteractions>();
        m_attack = GetComponent<PlayerAttack>();
    }

    private void OnEnable()
    {
        playerInputMap.Enable();
        playerInputMap.Movement.Movement.performed += controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled += controller.ReadDirection;
        playerInputMap.Attack.Attack.performed += m_animation.Attack;
        playerInputMap.Dash.Dash.performed += m_animation.Dash;
        playerInputMap.Interract.Interract.performed += m_interaction.Interract;
        playerInputMap.Attack.Throw.performed += ctx => m_attack.ThrowSpear();
    }

    private void OnDisable()
    {
        playerInputMap.Disable();
        playerInputMap.Movement.Movement.performed -= controller.ReadDirection;
        playerInputMap.Movement.Movement.canceled -= controller.ReadDirection;
        playerInputMap.Attack.Attack.performed -= m_animation.Attack;
        playerInputMap.Dash.Dash.performed -= m_animation.Dash;
        playerInputMap.Interract.Interract.performed -= m_interaction.Interract;
    }
}
