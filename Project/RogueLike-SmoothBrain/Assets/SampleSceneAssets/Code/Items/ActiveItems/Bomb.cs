using UnityEngine;

public class Bomb : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 5f;
    private const float throwStrength = 3f;
    private GameObject bombPf;
    public Bomb()
    {
        bombPf = Resources.Load<GameObject>("Bomb");
    }
    public void Activate()
    {
        GameObject player = Utilities.Player;
        Vector3 direction = player.transform.forward;
        var bomb = GameObject.Instantiate(bombPf, player.transform.position, Quaternion.identity);
        var explodingBomb = bomb.GetComponent<ExplodingBomb>();
        explodingBomb.ThrowToPos(Utilities.Hero, player.transform.position + direction * throwStrength, 0.5f);
        explodingBomb.Activate();
    } 
} 
