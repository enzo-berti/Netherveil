using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class KlopsDeathBehaviour : StateMachineBehaviour
{
    VisualEffect VFX;
    [SerializeField] float blastDiameter;
    [SerializeField] int blastDamage;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Explosion");
        VFX = animator.gameObject.transform.parent.GetComponent<KlopsStateMachine>().ExplodingVFX;
        Debug.Log("is VFX null => " + VFX == null);
        VFX.SetFloat("TimeToExplode", stateInfo.length);
        VFX.SetFloat("ExplosionTime", 2.5f);
        VFX.SetFloat("ExplosionRadius", blastDiameter);
        VFX.transform.position = animator.gameObject.transform.parent.position;
        VFX.Play();
        CoroutineManager.Instance.StartCustom(Explosion(animator.gameObject.transform.parent.GetComponent<KlopsStateMachine>(), stateInfo.length));
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CoroutineManager.Instance.StartCustom(BeforeDestroy(animator, VFX.GetFloat("ExplosionTime")));
    }

    private IEnumerator BeforeDestroy(Animator animator, float time)
    {
        animator.transform.parent.gameObject.SetActive(false);
        yield return new WaitForSeconds(time + 0.5f);
        Destroy(animator.transform.parent.parent.gameObject);
    }

    private IEnumerator Explosion(IAttacker attacker, float time)
    {
        yield return new WaitForSeconds(time);
        Physics.OverlapSphere(VFX.transform.transform.position, blastDiameter / 2f - blastDiameter / 8f, LayerMask.GetMask("Entity"))
            .Select(entity => entity.GetComponent<Hero>())
            .Where(entity => entity != null)
            .ToList()
            .ForEach(currentEntity =>
            {
                currentEntity.ApplyDamage(blastDamage, attacker);
            });
    }
}
