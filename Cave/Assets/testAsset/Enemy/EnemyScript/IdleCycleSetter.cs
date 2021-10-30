using System.Linq;
using UnityEngine;

public class IdleCycleSetter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("IdleOffset", Random.Range(0, 1.0f));
    }
}
