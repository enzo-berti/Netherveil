using UnityEngine;

public static class Utilities
{
    static public GameObject Player { get => GameObject.FindWithTag("Player"); }
    static public Hero Hero { get => Player.GetComponent<Hero>(); }
    static public PlayerInput PlayerInput { get => Player.GetComponent<PlayerInput>(); }
    static public PlayerController PlayerController { get => Player.GetComponent<PlayerController>(); }
    static public CharacterController CharacterController { get => Player.GetComponent<CharacterController>(); }
    static public Stats PlayerStat { get => Hero.Stats; }
    static public Inventory Inventory { get => Hero.Inventory; }

    static public bool IsPlayer(Entity entity)
    {
        return entity.CompareTag("Player");
    }
}
