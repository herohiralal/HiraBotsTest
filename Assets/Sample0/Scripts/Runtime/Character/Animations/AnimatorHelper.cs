using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

namespace AIEngineTest
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorHelper : MonoBehaviour
    {
        private static readonly int s_Speed = Animator.StringToHash("Speed");
        private static readonly int s_WeaponType = Animator.StringToHash("WeaponType");
        private static readonly int s_PlayMontage = Animator.StringToHash("PlayMontage");
        private static readonly int s_MontageType = Animator.StringToHash("MontageType");
        private static readonly int s_InterruptMontage = Animator.StringToHash("InterruptMontage");
        private static readonly int s_ActionNum = Animator.StringToHash("ActionNum");

        [SerializeField] private Animator m_Animator;
        [SerializeField] private UnityEvent m_OnFootL;
        [SerializeField] private UnityEvent m_OnFootR;
        [SerializeField] private UnityEvent m_OnHit;
        [SerializeField] private UnityEvent<MontageType> m_OnStateEnter;
        [SerializeField] private UnityEvent<MontageType> m_OnStateExit;
        [SerializeField] private UnityEvent<WeaponType> m_OnEquip;

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
                    m_Animator.SetTrigger(s_InterruptMontage);
                }

                if (value != MontageType.None)
                {
                    m_Animator.SetInteger(s_MontageType, (int) value);
                    m_Animator.SetTrigger(s_PlayMontage);
                }
            }
        }

        private WeaponType m_WeaponType;

        public WeaponType weaponType
        {
            get => m_WeaponType;
            private set
            {
                m_WeaponType = value;
                m_Animator.SetInteger(s_WeaponType, (int) value);
            }
        }

        public bool PrepareMeleeAttackR(int? value = null)
        {
            var max = -1;
            switch (m_WeaponType)
            {
                case WeaponType.None:
                    break;
                case WeaponType.Fists:
                    max = 3;
                    break;
                case WeaponType.Sword:
                case WeaponType.SwordAndShield:
                    max = 5;
                    break;
                case WeaponType.DualDaggers:
                    max = 2;
                    break;
                case WeaponType.Staff:
                    max = 6;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (max < 1)
            {
                return false;
            }

            actionNum = Mathf.Clamp(value.GetValueOrDefault(Random.Range(1, max + 1)), 1, max);
            return true;

        }

        public bool PrepareMeleeAttackL(int? value = null)
        {
            var max = -1;
            switch (m_WeaponType)
            {
                case WeaponType.None:
                case WeaponType.Sword:
                case WeaponType.Staff:
                    break;
                case WeaponType.Fists:
                    max = 3;
                    break;
                case WeaponType.SwordAndShield:
                    max = 1;
                    break;
                case WeaponType.DualDaggers:
                    max = 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (max < 1)
            {
                return false;
            }

            actionNum = Mathf.Clamp(value.GetValueOrDefault(Random.Range(1, max + 1)), 1, max);
            return true;
        }

        public bool PrepareToEquip(WeaponType? t = null)
        {
            var type = t ?? WeaponType.Fists;
            if (type == WeaponType.None || weaponType == type)
            {
                return false;
            }

            if (type == WeaponType.SwordAndShield && weaponType != WeaponType.Sword)
            {
                return false;
            }

            actionNum = (int) type;
            return true;
        }

        public bool PrepareToUnequip(WeaponType? t = null)
        {
            var type = t ?? weaponType;
            if (type == WeaponType.None || weaponType != type)
            {
                return false;
            }

            return true;
        }

        private static float GetMaxSpeedForCurrentWeaponType(WeaponType weaponType) => weaponType switch
        {
            WeaponType.None => 4.197f,
            WeaponType.Fists => 4.197f,
            WeaponType.Sword => 4.251f,
            WeaponType.SwordAndShield => 4.236f,
            WeaponType.DualDaggers => 4.251f,
            WeaponType.Staff => 5.084f,
            _ => throw new ArgumentOutOfRangeException(nameof(weaponType), weaponType, null)
        };

        public float speed
        {
            get => m_Animator.GetFloat(s_Speed) * GetMaxSpeedForCurrentWeaponType(m_WeaponType);
            set => m_Animator.SetFloat(s_Speed, value / GetMaxSpeedForCurrentWeaponType(m_WeaponType));
        }

        public int actionNum
        {
            get => m_Animator.GetInteger(s_ActionNum);
            set => m_Animator.SetInteger(s_ActionNum, value);
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

        [Preserve]
        public void Unsheathe()
        {
            weaponType = (WeaponType) actionNum;
            m_OnEquip.Invoke(m_WeaponType);
        }

        [Preserve]
        public void Sheathe()
        {
            weaponType = weaponType == WeaponType.SwordAndShield
                ? WeaponType.Sword
                : WeaponType.None;
            m_OnEquip.Invoke(weaponType);
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