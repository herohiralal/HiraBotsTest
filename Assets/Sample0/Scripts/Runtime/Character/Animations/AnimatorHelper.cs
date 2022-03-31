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
        private static readonly int s_KeepMontageActive = Animator.StringToHash("KeepMontageActive");
        private static readonly int s_Direction = Animator.StringToHash("Direction");
        private static readonly int s_GetUpFromRagdoll = Animator.StringToHash("GetUpFromRagdoll");

        [SerializeField] private Animator m_Animator;
        [SerializeField] private CharacterMeshWeaponSocketProvider m_CharacterMeshWeaponSocketProvider;
        [SerializeField] private UnityEvent m_OnFootL;
        [SerializeField] private UnityEvent m_OnFootR;
        [SerializeField] private UnityEvent m_OnHit;
        [SerializeField] private UnityEvent m_OnCast;
        [SerializeField] private UnityEvent<MontageType> m_OnStateEnter;
        [SerializeField] private UnityEvent<MontageType> m_OnStateExit;
        [SerializeField] private UnityEvent<EquipmentType> m_OnEquip;
        [SerializeField] private UnityEvent m_OnGetUpFromRagdoll;
        [SerializeField] private Collider[] m_RagdollColliders;

        public UnityEvent hit => m_OnHit;
        public UnityEvent<MontageType> stateEnter => m_OnStateEnter;
        public UnityEvent<MontageType> stateExit => m_OnStateExit;
        public UnityEvent<EquipmentType> equip => m_OnEquip;
        public UnityEvent getUpFromRagdoll => m_OnGetUpFromRagdoll;

        public float animatorSpeed
        {
            get => m_Animator.speed;
            set => m_Animator.speed = value;
        }

        private void Reset()
        {
            m_Animator = GetComponent<Animator>();
            m_CharacterMeshWeaponSocketProvider = GetComponent<CharacterMeshWeaponSocketProvider>();
        }

        #region Reactions

        public void CalculateDirection(Vector3 otherPosition)
        {
            var t = transform;
            var dir = (otherPosition - t.position).normalized;
            var ang = Vector3.SignedAngle(t.forward, dir, t.up) % 360;
            ang += ang < 0 ? 360 : 0;
            m_Animator.SetFloat(s_Direction, ang);
        }

        #endregion

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

        public bool keepMontageActive
        {
            get => m_Animator.GetBool(s_KeepMontageActive);
            set => m_Animator.SetBool(s_KeepMontageActive, value);
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
                    max = 5;
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

        public bool PrepareToEquip()
        {
            actionNum = (int) m_OwnedEquipmentType;
            return true;
        }

        public bool PrepareToUnequip()
        {
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

        #region Ragdoll

        private bool ragdollCollidersEnabled
        {
            get => !m_RagdollColliders[0].isTrigger;
            set
            {
                foreach (var col in m_RagdollColliders)
                {
                    col.isTrigger = !value;

                    var rb = col.attachedRigidbody;
                    rb.useGravity = value;
                    rb.isKinematic = !value;
                }
            }
        }

        public void TriggerRagdollOn()
        {
            m_Animator.enabled = false;

            if (m_CurrentMontageState != MontageType.None)
            {
                OnStateExit(m_CurrentMontageState);
            }

            ragdollCollidersEnabled = true;
        }

        public void TriggerRagdollOff()
        {
            m_Animator.enabled = true;
            m_Animator.SetTrigger(s_GetUpFromRagdoll);

            ragdollCollidersEnabled = false;
        }

        public void GetUpFromRagdoll()
        {
            m_OnGetUpFromRagdoll.Invoke();
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
        public void Cast()
        {
            m_OnCast.Invoke();
        }

        [Preserve]
        public void Unsheathe()
        {
            var et = (EquipmentType) actionNum;

            if (et == EquipmentType.SwordAndShield && equipmentType != EquipmentType.Sword)
            {
                et = EquipmentType.Sword;
            }

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