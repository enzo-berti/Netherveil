using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class KlopsDeathBehaviour : StateMachineBehaviour
{
    VisualEffect VFX;
    [SerializeField] float blastDiameter;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Explosion");
        VFX = animator.gameObject.transform.parent.GetComponent<KlopsStateMachine>().ExplodingVFX;
        Debug.Log("is VFX null => " + VFX == null);
        VFX.SetFloat("TimeToExplode", stateInfo.length);
        VFX.SetFloat("ExplosionTime", 1f);
        VFX.SetFloat("ExplosionRadius", blastDiameter);
        VFX.transform.position = animator.gameObject.transform.parent.position;
        VFX.Play();
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CoroutineManager.Instance.StartCustom(BeforeDestroy(animator, VFX.GetFloat("ExplosionTime")));
    }

    private IEnumerator BeforeDestroy(Animator animator, float time)
    {
        animator.transform.parent.gameObject.SetActive(false);
        yield return new WaitForSeconds(time);
        Destroy(animator.transform.parent.parent.gameObject);
    }
}
