using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Bomb : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 5f;
    private GameObject bombPf;
    public Bomb()
    {
        bombPf = Resources.Load<GameObject>("Bomb");
    }
    public void Activate()
    {
        GameObject player = Utilities.Player;
        player.GetComponent<PlayerController>().MouseOrientation();
        Vector3 direction = player.transform.forward;
        var bomb = GameObject.Instantiate(bombPf, player.transform.position, Quaternion.identity);
        var explodingBomb = bomb.GetComponent<ExplodingBomb>();
        explodingBomb.ThrowToPos(Utilities.Hero, player.transform.position + direction * Utilities.Hero.Stats.GetValue(Stat.ATK_RANGE), 0.5f);
        explodingBomb.SetTimeToExplode(0.5f * 1.5f);
        explodingBomb.SetBlastDamages((int)Utilities.Hero.Stats.GetValue(Stat.ATK));
        explodingBomb.Activate();
    } 
} 
