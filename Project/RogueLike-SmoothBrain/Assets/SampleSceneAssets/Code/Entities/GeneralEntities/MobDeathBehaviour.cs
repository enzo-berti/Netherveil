using UnityEngine;
using UnityEngine.VFX;

public class MobDeathBehaviour : StateMachineBehaviour
{

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject.Destroy(GameObject.Instantiate(GameResources.Get<GameObject>("VFX_Death"), animator.transform.parent.position, Quaternion.identity), 3f);
        Destroy(animator.transform.parent.parent.gameObject);
    }
}
