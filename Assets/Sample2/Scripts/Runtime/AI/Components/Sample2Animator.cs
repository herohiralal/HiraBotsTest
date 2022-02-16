using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace AIEngineTest
{
    [System.Serializable, UnityEngine.AI.ExposedToHiraBots("D8D1DA80-E3F8-4899-9DBD-ADB97DBC52A8")]
    public enum Sample2MontageType
    {
        None = 0,
        MeleeAttackRight = 1,
        MeleeAttackLeft = 2
    }

    [System.Serializable, UnityEngine.AI.ExposedToHiraBots("24810CDF-70F7-4906-8B1D-FA60DADC1CEA")]
    public enum Sample2WeaponType
    {
        None = -1,
        Fists = 0,
    }

    [RequireComponent(typeof(Animator))]
    public class Sample2Animator : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private UnityEvent m_OnFootL;
        [SerializeField] private UnityEvent m_OnFootR;
        [SerializeField] private UnityEvent m_OnHit;
        [SerializeField] private UnityEvent<Sample2MontageType> m_OnStateEnter;
        [SerializeField] private UnityEvent<Sample2MontageType> m_OnStateExit;

        public UnityEvent<Sample2MontageType> stateEnter => m_OnStateEnter;
        public UnityEvent<Sample2MontageType> stateExit => m_OnStateExit;

        private void Reset()
        {
            m_Animator = GetComponent<Animator>();
        }

        private Sample2MontageType m_CurrentMontageState = Sample2MontageType.None;
        public Sample2MontageType currentMontageState
        {
            get => m_CurrentMontageState;
            set
            {
                if (m_CurrentMontageState != Sample2MontageType.None)
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

        private Sample2WeaponType m_WeaponType = Sample2WeaponType.None;

        public Sample2WeaponType weaponType
        {
            get => m_WeaponType;
            set
            {
                m_Animator.SetInteger(AnimatorHashes.s_WeaponType, (int) value);
                m_WeaponType = value;
            }
        }

        public int actionNum
        {
            get => m_Animator.GetInteger(AnimatorHashes.s_ActionNum);
            set => m_Animator.SetInteger(AnimatorHashes.s_ActionNum, value);
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

        [Preserve]
        public void Hit()
        {
            m_OnHit.Invoke();
        }

        public void OnStateEnter(Sample2MontageType montageType)
        {
            UnityEngine.Assertions.Assert.AreNotEqual(Sample2MontageType.None, montageType);
            UnityEngine.Assertions.Assert.AreNotEqual(m_CurrentMontageState, montageType);
            m_CurrentMontageState = montageType;
            m_OnStateEnter.Invoke(montageType);
        }

        public void OnStateExit(Sample2MontageType montageType)
        {
            UnityEngine.Assertions.Assert.AreNotEqual(Sample2MontageType.None, montageType);
            UnityEngine.Assertions.Assert.AreEqual(m_CurrentMontageState, montageType);
            m_CurrentMontageState = Sample2MontageType.None;
            m_OnStateExit.Invoke(montageType);
        }
    }
}