using UnityEngine;

public static class Utilities
{
    static public GameObject Player { get => GameObject.FindWithTag("Player"); }

    static public Hero Hero { get => Player.TryGetComponent(out Hero hero) ? hero : null; }
    static public Stats PlayerStat { get => Hero.Stats; }
    static public Inventory Inventory { get => Hero.Inventory; }
    
    static public PlayerInput PlayerInput { get => Player.TryGetComponent(out PlayerInput playerInput) ? playerInput : null; }
    static public PlayerController PlayerController { get => Player.TryGetComponent(out PlayerController playerController) ? playerController : null; }
    static public CharacterController CharacterController { get => Player.TryGetComponent(out CharacterController characterController) ? characterController : null; }

    static public bool IsPlayer(Entity entity)
    {
        return entity.CompareTag("Player");
    }
}
