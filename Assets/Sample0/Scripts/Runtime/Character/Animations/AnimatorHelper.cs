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
        [SerializeField] private CharacterMeshWeaponSocketProvider m_CharacterMeshWeaponSocketProvider;
        [SerializeField] private UnityEvent m_OnFootL;
        [SerializeField] private UnityEvent m_OnFootR;
        [SerializeField] private UnityEvent m_OnHit;
        [SerializeField] private UnityEvent<MontageType> m_OnStateEnter;
        [SerializeField] private UnityEvent<MontageType> m_OnStateExit;
        [SerializeField] private UnityEvent<EquipmentType> m_OnEquip;

        public UnityEvent<MontageType> stateEnter => m_OnStateEnter;
        public UnityEvent<MontageType> stateExit => m_OnStateExit;

        private void Reset()
        {
            m_Animator = GetComponent<Animator>();
            m_CharacterMeshWeaponSocketProvider = GetComponent<CharacterMeshWeaponSocketProvider>();
        }

        #region Montage

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

        public int actionNum
        {
            get => m_Animator.GetInteger(s_ActionNum);
            set => m_Animator.SetInteger(s_ActionNum, value);
        }

        #endregion

        #region Attack

        public bool PrepareMeleeAttackR(int? value = null)
        {
            var max = -1;
            switch (m_CurrentEquipmentType)
            {
                case EquipmentType.None:
                    break;
                case EquipmentType.Fists:
                    max = 3;
                    break;
                case EquipmentType.Sword:
                case EquipmentType.SwordAndShield:
                    max = 5;
                    break;
                case EquipmentType.DualDaggers:
                    max = 2;
                    break;
                case EquipmentType.Staff:
                    max = 6;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
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
            switch (m_CurrentEquipmentType)
            {
                case EquipmentType.None:
                case EquipmentType.Sword:
                case EquipmentType.Staff:
                    break;
                case EquipmentType.Fists:
                    max = 3;
                    break;
                case EquipmentType.SwordAndShield:
                    max = 1;
                    break;
                case EquipmentType.DualDaggers:
                    max = 2;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            if (max < 1)
            {
                return false;
            }

            actionNum = Mathf.Clamp(value.GetValueOrDefault(Random.Range(1, max + 1)), 1, max);
            return true;
        }

        #endregion

        #region Equipment

        private EquipmentType m_OwnedEquipmentType;
        private EquipmentType m_CurrentEquipmentType;

        public void DropAllEquipment()
        {
            void DropEquipmentFromSocket(ref CharacterMeshWeaponSocketProvider.Socket socket)
            {
                if (socket.currentlyAttached != null)
                {
                    GameManager.weaponGenerator.Discard(socket.Detach());
                }
            }

            void DropEquipmentFromSockets(ref CharacterMeshWeaponSocketProvider.WeaponSockets sockets)
            {
                DropEquipmentFromSocket(ref sockets.m_SheathedSocket);
                DropEquipmentFromSocket(ref sockets.m_EquippedSocket);
            }

            DropEquipmentFromSockets(ref m_CharacterMeshWeaponSocketProvider.m_SwordSockets);
            DropEquipmentFromSockets(ref m_CharacterMeshWeaponSocketProvider.m_ShieldSockets);
            DropEquipmentFromSockets(ref m_CharacterMeshWeaponSocketProvider.m_DaggerLSockets);
            DropEquipmentFromSockets(ref m_CharacterMeshWeaponSocketProvider.m_DaggerRSockets);
            DropEquipmentFromSockets(ref m_CharacterMeshWeaponSocketProvider.m_StaffSockets);

            m_OwnedEquipmentType = EquipmentType.None;
            equipmentType = EquipmentType.None;
        }

        public void InitializeEquipment(EquipmentType type)
        {
            DropAllEquipment();

            switch (type)
            {
                case EquipmentType.None:
                case EquipmentType.Fists:
                    break;
                case EquipmentType.Sword:
                    m_CharacterMeshWeaponSocketProvider.m_SwordSockets.m_SheathedSocket.Attach(GameManager.weaponGenerator.GenerateSword());
                    break;
                case EquipmentType.SwordAndShield:
                    m_CharacterMeshWeaponSocketProvider.m_SwordSockets.m_SheathedSocket.Attach(GameManager.weaponGenerator.GenerateSword());
                    m_CharacterMeshWeaponSocketProvider.m_ShieldSockets.m_SheathedSocket.Attach(GameManager.weaponGenerator.GenerateShield());
                    break;
                case EquipmentType.DualDaggers:
                    m_CharacterMeshWeaponSocketProvider.m_DaggerLSockets.m_SheathedSocket.Attach(GameManager.weaponGenerator.GenerateDagger());
                    m_CharacterMeshWeaponSocketProvider.m_DaggerRSockets.m_SheathedSocket.Attach(GameManager.weaponGenerator.GenerateDagger());
                    break;
                case EquipmentType.Staff:
                    m_CharacterMeshWeaponSocketProvider.m_StaffSockets.m_SheathedSocket.Attach(GameManager.weaponGenerator.GenerateStaff());
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(type), type, null);
            }

            m_OwnedEquipmentType = type;
        }

        public EquipmentType ownedEquipment => m_OwnedEquipmentType;

        public EquipmentType equipmentType
        {
            get => m_CurrentEquipmentType;
            private set
            {
                m_CurrentEquipmentType = value;
                m_Animator.SetInteger(s_WeaponType, (int) value);
            }
        }

        public bool PrepareToEquip(EquipmentType? t = null)
        {
            var type = t ?? EquipmentType.Fists;
            if (type == EquipmentType.None || m_CurrentEquipmentType == type)
            {
                return false;
            }

            if (type == EquipmentType.SwordAndShield && m_CurrentEquipmentType != EquipmentType.Sword)
            {
                return false;
            }

            switch (type)
            {
                case EquipmentType.Fists:
                    break;
                case EquipmentType.Sword when m_OwnedEquipmentType is EquipmentType.Sword or EquipmentType.SwordAndShield:
                    break;
                case EquipmentType.SwordAndShield when m_OwnedEquipmentType is EquipmentType.SwordAndShield:
                    break;
                case EquipmentType.DualDaggers when m_OwnedEquipmentType is EquipmentType.DualDaggers:
                    break;
                case EquipmentType.Staff when m_OwnedEquipmentType is EquipmentType.Staff:
                    break;
                default:
                    return false;
            }

            actionNum = (int) type;
            return true;
        }

        public bool PrepareToUnequip(EquipmentType? t = null)
        {
            var type = t ?? m_CurrentEquipmentType;
            if (type == EquipmentType.None || m_CurrentEquipmentType != type)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Locomotion

        private static float GetMaxSpeedForCurrentWeaponType(EquipmentType equipmentType) => equipmentType switch
        {
            EquipmentType.None => 4.197f,
            EquipmentType.Fists => 4.197f,
            EquipmentType.Sword => 4.251f,
            EquipmentType.SwordAndShield => 4.236f,
            EquipmentType.DualDaggers => 4.251f,
            EquipmentType.Staff => 5.084f,
            _ => throw new System.ArgumentOutOfRangeException(nameof(equipmentType), equipmentType, null)
        };

        public float speed
        {
            get => m_Animator.GetFloat(s_Speed) * GetMaxSpeedForCurrentWeaponType(m_CurrentEquipmentType);
            set => m_Animator.SetFloat(s_Speed, value / GetMaxSpeedForCurrentWeaponType(m_CurrentEquipmentType));
        }

        #endregion

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
            var et = (EquipmentType) actionNum;
            equipmentType = et;

            switch (et)
            {
                case EquipmentType.None:
                    break;
                case EquipmentType.Fists:
                    break;
                case EquipmentType.Sword:
                    m_CharacterMeshWeaponSocketProvider.m_SwordSockets.Unsheathe();
                    break;
                case EquipmentType.SwordAndShield:
                    m_CharacterMeshWeaponSocketProvider.m_ShieldSockets.Unsheathe();
                    break;
                case EquipmentType.DualDaggers:
                    m_CharacterMeshWeaponSocketProvider.m_DaggerLSockets.Unsheathe();
                    m_CharacterMeshWeaponSocketProvider.m_DaggerRSockets.Unsheathe();
                    break;
                case EquipmentType.Staff:
                    m_CharacterMeshWeaponSocketProvider.m_StaffSockets.Unsheathe();
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
            
            m_OnEquip.Invoke(m_CurrentEquipmentType);
        }

        [Preserve]
        public void Sheathe()
        {
            var et = m_CurrentEquipmentType;
            equipmentType = m_CurrentEquipmentType == EquipmentType.SwordAndShield
                ? EquipmentType.Sword
                : EquipmentType.None;

            switch (et)
            {
                case EquipmentType.None:
                    break;
                case EquipmentType.Fists:
                    break;
                case EquipmentType.Sword:
                    m_CharacterMeshWeaponSocketProvider.m_SwordSockets.Sheathe();
                    break;
                case EquipmentType.SwordAndShield:
                    m_CharacterMeshWeaponSocketProvider.m_ShieldSockets.Sheathe();
                    break;
                case EquipmentType.DualDaggers:
                    m_CharacterMeshWeaponSocketProvider.m_DaggerLSockets.Sheathe();
                    m_CharacterMeshWeaponSocketProvider.m_DaggerRSockets.Sheathe();
                    break;
                case EquipmentType.Staff:
                    m_CharacterMeshWeaponSocketProvider.m_StaffSockets.Sheathe();
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            m_OnEquip.Invoke(m_CurrentEquipmentType);
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