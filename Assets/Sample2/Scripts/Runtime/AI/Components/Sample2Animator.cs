using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace AIEngineTest
{
    [System.Serializable]
    public enum Sample2MontageType
    {
        None = 0,
        MeleeAttack = 1,
    }

    [RequireComponent(typeof(Animator))]
    public class Sample2Animator : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private UnityEvent m_OnFootL;
        [SerializeField] private UnityEvent m_OnFootR;
        [SerializeField] private UnityEvent<Sample2MontageType> m_OnStateEnter;
        [SerializeField] private UnityEvent<Sample2MontageType> m_OnStateExit;

        private void Reset()
        {
            m_Animator = GetComponent<Animator>();
        }

        private Sample2MontageType m_CurrentState = Sample2MontageType.None;
        public Sample2MontageType currentState
        {
            get => m_CurrentState;
            set
            {
                if (m_CurrentState != Sample2MontageType.None)
                {
                    m_Animator.SetTrigger(AnimatorHashes.s_InterruptMontage);
                }

                if (value != Sample2MontageType.None)
                {
                    m_Animator.SetInteger(AnimatorHashes.s_MontageType, (int) value);
                    m_Animator.SetTrigger(AnimatorHashes.s_PlayMontage);
                }
            }
        }

        [Preserve]
        public void FootL()
        {
            m_OnFootL.Invoke();
        }

        [Preserve]
        public void FootR()
        {
            m_OnFootR.Invoke();
        }

        public void OnStateEnter(Sample2MontageType montageType)
        {
            UnityEngine.Assertions.Assert.AreNotEqual(Sample2MontageType.None, montageType);
            UnityEngine.Assertions.Assert.AreNotEqual(m_CurrentState, montageType);
            m_CurrentState = montageType;
            m_OnStateEnter.Invoke(montageType);
        }

        public void OnStateExit(Sample2MontageType montageType)
        {
            UnityEngine.Assertions.Assert.AreNotEqual(Sample2MontageType.None, montageType);
            UnityEngine.Assertions.Assert.AreEqual(m_CurrentState, montageType);
            m_CurrentState = Sample2MontageType.None;
            m_OnStateExit.Invoke(montageType);
        }
    }
}