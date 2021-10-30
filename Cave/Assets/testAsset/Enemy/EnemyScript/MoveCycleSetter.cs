using System.Linq;
using UnityEngine;

public class MoveCycleSetter : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("MoveOffset", Random.Range(0, 1.0f));
    }
}