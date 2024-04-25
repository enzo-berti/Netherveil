using UnityEngine;

public class Bomb : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 5f;
    private GameObject bombPf;

#pragma warning disable CS0414 // Supprimer le warning dans unity
#pragma warning disable IDE0052 // Supprimer les membres privés non lus
    //used to display in description, dont delete it
    readonly float displayValue;
#pragma warning restore IDE0052
#pragma warning restore CS0414

    public Bomb()
    {
        bombPf = GameResources.Get<GameObject>("Bomb");
        displayValue = Utilities.Hero.Stats.GetValue(Stat.ATK);
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
