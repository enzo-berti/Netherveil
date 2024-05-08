using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class Bomb : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 10f;
    public static bool bombIsThrow;
    private GameObject bombPf;
    private const float deltaY = 0.5f;

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
        player.GetComponent<PlayerController>().PlayLaunchBombAnim();
        player.GetComponent<PlayerController>().RotatePlayerToDeviceAndMargin();

        CoroutineManager.Instance.StartCustom(WaitLaunch(player));
        
    }

    private IEnumerator WaitLaunch(GameObject player)
    {
        Transform hand = player.GetComponent<PlayerController>().LeftHandTransform;
        Vector3 direction = player.transform.forward;
        var bomb = GameObject.Instantiate(bombPf, hand);
        var explodingBomb = bomb.GetComponent<ExplodingBomb>();

        yield return new WaitUntil(() => bombIsThrow);

        explodingBomb.gameObject.transform.parent = null;
        explodingBomb.gameObject.transform.rotation = Quaternion.identity;
        explodingBomb.ThrowToPos(Utilities.Hero, player.transform.position + direction * Utilities.Hero.Stats.GetValue(Stat.ATK_RANGE), 0.5f);
        explodingBomb.SetTimeToExplode(0.5f * 1.5f);

        explodingBomb.SetBlastDamages((int)Utilities.Hero.Stats.GetValue(Stat.ATK) * 2);
        explodingBomb.Activate();
        bombIsThrow = false;
    }
}
