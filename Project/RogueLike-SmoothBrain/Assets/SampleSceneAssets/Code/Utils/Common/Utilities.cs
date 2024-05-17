using UnityEngine;

public static class Utilities
{
    static public GameObject Player { get => GameObject.FindWithTag("Player"); }

    static public Hero Hero { get => Player != null ? Player.TryGetComponent(out Hero hero) ? hero : null : null; }
    static public Stats PlayerStat { get => Hero.Stats; }
    static public Inventory Inventory { get => Hero.Inventory; }
    
    static public PlayerInput PlayerInput { get => Player != null ? Player.TryGetComponent(out PlayerInput playerInput) ? playerInput : null : null; }
    static public PlayerController PlayerController { get => Player != null ? Player.TryGetComponent(out PlayerController playerController) ? playerController : null : null; }
    static public CharacterController CharacterController { get => Player != null ? Player.TryGetComponent(out CharacterController characterController) ? characterController : null : null; }

    static public bool IsPlayer(Entity entity)
    {
        return entity.CompareTag("Player");
    }
}
