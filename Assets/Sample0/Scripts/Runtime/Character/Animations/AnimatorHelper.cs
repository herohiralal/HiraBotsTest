using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace AIEngineTest
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorHelper : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;
        [SerializeField] private UnityEvent m_OnFootL;
        [SerializeField] private UnityEvent m_OnFootR;
        [SerializeField] private UnityEvent m_OnHit;
        [SerializeField] private UnityEvent<MontageType> m_OnStateEnter;
        [SerializeField] private UnityEvent<MontageType> m_OnStateExit;

        public UnityEvent<MontageType> stateEnter => m_OnStateEnter;
        public UnityEvent<MontageType> stateExit => m_OnStateExit;

        private void Reset()
        {
            m_Animator = GetComponent<Animator>();
        }

        private MontageType m_CurrentMontageState = MontageType.None;
        public MontageType currentMontageState
        {
            get => m_CurrentMontageState;
            set
            {
                if (m_CurrentMontageState != MontageType.None)
                {
                    m_Animator.SetTrigger(AnimatorHashes.s_InterruptMontage);
                }

                if (value != MontageType.None)
                {
                    m_Animator.SetInteger(AnimatorHashes.s_MontageType, (int) value);
                    m_Animator.SetTrigger(AnimatorHashes.s_PlayMontage);
                }
            }
        }

        private WeaponType m_WeaponType = WeaponType.None;

        public WeaponType weaponType
        {
            get => m_WeaponType;
            set
            {
                m_Animator.SetInteger(AnimatorHashes.s_WeaponType, (int) value);
                m_WeaponType = value;
            }
        }

        public float speed
        {
            get => m_Animator.GetFloat(AnimatorHashes.s_Speed) * AnimatorConstants.GetMaxSpeed(m_WeaponType);
            set => m_Animator.SetFloat(AnimatorHashes.s_Speed, value / AnimatorConstants.GetMaxSpeed(m_WeaponType));
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

        public void OnStateEnter(MontageType montageType)
        {
            UnityEngine.Assertions.Assert.AreNotEqual(MontageType.None, montageType);
            UnityEngine.Assertions.Assert.AreNotEqual(m_CurrentMontageState, montageType);
            m_CurrentMontageState = montageType;
            m_OnStateEnter.Invoke(montageType);
        }

        public void OnStateExit(MontageType montageType)
        {
            UnityEngine.Assertions.Assert.AreNotEqual(MontageType.None, montageType);
            UnityEngine.Assertions.Assert.AreEqual(m_CurrentMontageState, montageType);
            m_CurrentMontageState = MontageType.None;
            m_OnStateExit.Invoke(montageType);
        }
    }
}