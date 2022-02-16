using UnityEngine;

namespace AIEngineTest
{
    [SharedBetweenAnimators]
    public class AnimatorMontageStateBroadcaster : StateMachineBehaviour
    {
        [SerializeField] private MontageType m_MontageType = MontageType.None;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<AnimatorHelper>().OnStateEnter(m_MontageType);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<AnimatorHelper>().OnStateExit(m_MontageType);
        }
    }
}