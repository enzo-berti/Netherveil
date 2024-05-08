using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class Bomb : ItemEffect, IActiveItem
{
    public float Cooldown { get; set; } = 10f;
    public static bool bombIsThrow;
    private GameObject bombPf;
    private const float deltaY = 0.5f;
    readonly int damages = 15;

    public Bomb()
    {
        bombPf = GameResources.Get<GameObject>("Bomb");
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

        explodingBomb.SetBlastDamages(damages);
        explodingBomb.Activate();
        bombIsThrow = false;
    }
}
