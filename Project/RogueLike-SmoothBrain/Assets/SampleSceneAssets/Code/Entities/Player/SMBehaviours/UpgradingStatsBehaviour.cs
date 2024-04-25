using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradingStatsBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Utilities.Hero.Stats.GetValue(Stat.CORRUPTION) == Utilities.Hero.Stats.GetMaxValue(Stat.CORRUPTION))
        {
            HudHandler.current.DescriptionTab.SetTab("Damnation Veil", "On activation, creates a damnation zone that applies the damnation effect that doubles the damages received to all enemies touched by the zone.");
            HudHandler.current.DescriptionTab.OpenTab();
        }
        else if (Utilities.Hero.Stats.GetValue(Stat.CORRUPTION) == Utilities.Hero.Stats.GetMinValue(Stat.CORRUPTION))
        {
            HudHandler.current.DescriptionTab.SetTab("Divine Shield", "On activation, creates a shield around you that nullifies damages for a small amount of time.");
            HudHandler.current.DescriptionTab.OpenTab();
        }

        GameObject.FindWithTag("Player").GetComponent<PlayerInput>().EnableGameplayInputs();
        GameObject.FindWithTag("Player").GetComponent<Hero>().State = (int)Entity.EntityState.MOVE;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
