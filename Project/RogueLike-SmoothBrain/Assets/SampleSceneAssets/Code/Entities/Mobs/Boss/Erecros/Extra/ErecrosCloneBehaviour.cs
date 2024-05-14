using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class ErecrosCloneBehaviour : MonoBehaviour
{
    VisualEffect VFXBomb;
    IAttacker attacker;

    CameraUtilities cameraUtilities;

    [SerializeField] Collider collider;

    private void Awake()
    {
        VFXBomb = GetComponentInChildren<VisualEffect>();

        cameraUtilities = Camera.main.GetComponent<CameraUtilities>();
    }

    public void Explode(IAttacker _attacker)
    {
        StartCoroutine(ExplosionCoroutine());
    }

    IEnumerator ExplosionCoroutine()
    {
        VFXBomb.Play();

        Hero player = Utilities.Hero;
        float timer = 0f;
        float timeToExplode = VFXBomb.GetFloat("TimeToExplode");
        float explosionDuration = VFXBomb.GetFloat("ExplosionTime");

        Object.Destroy(transform.parent.gameObject, timeToExplode + explosionDuration);

        while (timer < timeToExplode)
        {
            timer += Time.deltaTime;

            LookAtPlayer(player.transform);
            transform.position += (player.transform.position - transform.position).normalized * 5f * Time.deltaTime;

            yield return null;
        }

        bool playerHit = false;

        GetComponentInChildren<Renderer>().gameObject.SetActive(false);

        DeviceManager.Instance.ApplyVibrations(0.8f, 0.8f, 0.5f);
        cameraUtilities.ShakeCamera(0.5f, 0.5f, EasingFunctions.EaseInQuint);

        Vector3 clonePos = transform.position;
        clonePos.y = player.transform.position.y;

        timer = 0f;
        do
        {
            timer += Time.deltaTime;

            if (Vector3.Distance(player.transform.position, clonePos) <= VFXBomb.GetFloat("ExplosionRadius") / 2f)
            {
                player.ApplyDamage(10, attacker);
                playerHit = true;
            }
        } while (timer < explosionDuration && !playerHit);

        yield return null;
    }

    public void LookAtPlayer(Transform _player)
    {
        Vector3 mobToPlayer = _player.position - transform.position;
        mobToPlayer.y = 0f;

        Quaternion lookRotation = Quaternion.LookRotation(mobToPlayer);
        lookRotation.x = 0;
        lookRotation.z = 0;

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
    }

    public bool AttackCollide(IAttacker _attacker, bool debugMode = true)
    {
        if (debugMode)
        {
            collider.gameObject.SetActive(true);
        }

        Vector3 rayOffset = Vector3.up / 2;

        Collider[] tab = PhysicsExtensions.CheckAttackCollideRayCheck(collider, transform.position + rayOffset, "Player", LayerMask.GetMask("Map"));
        if (tab.Length > 0)
        {
            foreach (Collider col in tab)
            {
                if (col.gameObject.GetComponent<Hero>() != null)
                {
                    IDamageable damageable = col.gameObject.GetComponent<IDamageable>();
                    _attacker.Attack(damageable);

                    if (debugMode)
                    {
                        collider.gameObject.SetActive(false);
                    }
                    return true;
                }
            }
        }
        return false;
    }

    public void DisableDebugCollider()
    {
        collider.gameObject.SetActive(false);
    }
}
