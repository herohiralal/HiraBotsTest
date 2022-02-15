using UnityEngine;

namespace AIEngineTest
{
    public class Sample2AnimatorMontageStateBroadcaster : StateMachineBehaviour
    {
        [SerializeField] private Sample2MontageType m_MontageType = Sample2MontageType.None;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<Sample2Animator>().OnStateEnter(m_MontageType);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<Sample2Animator>().OnStateExit(m_MontageType);
        }
    }
}