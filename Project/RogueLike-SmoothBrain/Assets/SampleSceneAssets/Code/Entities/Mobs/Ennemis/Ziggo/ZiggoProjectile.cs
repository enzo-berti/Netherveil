using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.WSA;

public class ZiggoProjectile : MonoBehaviour
{
    Hero player;
    float flaqueRadius;
    bool hitGround;
    float effectCooldown;

    private void OnEnable()
    {
        player = Utilities.Hero;
        hitGround = false;
    }

    private void Update()
    {
        if (effectCooldown <= 0)
        {
            if (Vector3.SqrMagnitude(player.transform.position - transform.position) < 2f * 2f)
            {
                // apply status
                player.AddStatus(new Poison(2f, 1));
                effectCooldown = 0.5f;
            }
        }
    }

    public void ThrowToPos(IAttacker attacker, Vector3 pos, float throwTime)
    {
        StartCoroutine(ThrowToPosCoroutine(pos, throwTime));
    }

    private IEnumerator ThrowToPosCoroutine(Vector3 pos, float throwTime)
    {
        float timer = 0;
        Vector3 basePos = this.transform.position;
        Vector3 position3D = Vector3.zero;
        float a = -16, b = 16;
        float c = this.transform.position.y;
        float timerToReach = MathsExtension.Resolve2ndDegree(a, b, c, 0).Max();
        while (timer < timerToReach)
        {
            yield return null;
            timer = timer > timerToReach ? timerToReach : timer;
            if (timer < 1.0f)
            {
                timer = timer > 1 ? 1 : timer;

                position3D = Vector3.Lerp(basePos, pos, timer);
            }
            position3D.y = MathsExtension.SquareFunction(a, b, c, timer);
            this.transform.position = position3D;
            timer += Time.deltaTime / throwTime;
        }
    }
}
