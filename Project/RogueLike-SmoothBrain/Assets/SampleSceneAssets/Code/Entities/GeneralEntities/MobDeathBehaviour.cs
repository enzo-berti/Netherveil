using UnityEngine;
using UnityEngine.VFX;

public class MobDeathBehaviour : StateMachineBehaviour
{

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime > 0.9f)
        {
            GameObject.Destroy(GameObject.Instantiate(GameResources.Get<GameObject>("VFX_Death"), animator.transform.parent.position, Quaternion.identity), 3f);
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        Destroy(animator.transform.parent.parent.gameObject);
    }
}
