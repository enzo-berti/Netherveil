using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Transition : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public WaitForSeconds ToggleTransition(bool toggle)
    {
        animator.SetBool("IsStart", toggle);
        return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
