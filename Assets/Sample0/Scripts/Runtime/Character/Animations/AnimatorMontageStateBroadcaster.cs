using UnityEngine;

namespace AIEngineTest
{
    [SharedBetweenAnimators]
    public class AnimatorMontageStateBroadcaster : StateMachineBehaviour
    {
        [SerializeField] private MontageType m_MontageType = MontageType.None;

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            animator.GetComponent<AnimatorHelper>().OnStateEnter(m_MontageType);
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            animator.GetComponent<AnimatorHelper>().OnStateExit(m_MontageType);
        }
    }
}