using UnityEngine;

namespace AIEngineTest
{
    [SharedBetweenAnimators]
    public class AnimatorDisableRagdollTrigger : StateMachineBehaviour
    {
        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            animator.GetComponent<AnimatorHelper>().GetUpFromRagdoll();
        }
    }
}