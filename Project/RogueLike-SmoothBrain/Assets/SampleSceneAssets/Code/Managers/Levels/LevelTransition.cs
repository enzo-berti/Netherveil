using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LevelTransition : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public async Task PlayTransition()
    {
        animator.SetTrigger("Start");
        await Task.Delay((int)(animator.GetCurrentAnimatorStateInfo(0).length * 1100));
    }
}
