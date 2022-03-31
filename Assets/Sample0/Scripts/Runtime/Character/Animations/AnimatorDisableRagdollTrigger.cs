using UnityEngine;

namespace AIEngineTest
{
    [SharedBetweenAnimators]
    public class AnimatorDisableRagdollTrigger : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<AnimatorHelper>().GetUpFromRagdoll();
        }
    }
}