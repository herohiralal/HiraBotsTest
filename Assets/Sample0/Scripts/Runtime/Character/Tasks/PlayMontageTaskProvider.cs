using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public struct InterruptMontageMessage
    {
    }

    public class PlayMontageTask : IHiraBotsTask, IMessageListener<InterruptMontageMessage>
    {
        private PlayMontageTask()
        {
        }

        private AnimatorHelper m_AnimatorHelper;
        private MontageType m_Type;
        private int? m_IntegerParam;
        private EquipmentType? m_WeaponTypeParam;
        private HiraBotsTaskResult m_Status;

        public static PlayMontageTask Get(AnimatorHelper animatorHelper, MontageType type, EquipmentType? weaponTypeParam, int? integerParam)
        {
            var output = s_Executables.Count == 0 ? new PlayMontageTask() : s_Executables.Pop();
            output.m_AnimatorHelper = animatorHelper;
            output.m_Type = type;
            output.m_Status = HiraBotsTaskResult.InProgress;
            output.m_WeaponTypeParam = weaponTypeParam;
            output.m_IntegerParam = integerParam;
            return output;
        }

        private static readonly Stack<PlayMontageTask> s_Executables = new Stack<PlayMontageTask>();

        public void Begin()
        {
            m_AnimatorHelper.stateExit.AddListener(OnAnimatorStateExit);

            switch (m_Type)
            {
                case MontageType.None:
                    m_Status = HiraBotsTaskResult.Failed;
                    return; // no need to run the montage
                case MontageType.MeleeAttackRight:
                    if (!m_AnimatorHelper.PrepareMeleeAttackR(m_IntegerParam))
                    {
                        m_Status = HiraBotsTaskResult.Failed;
                        return;
                    }
                    break;
                case MontageType.MeleeAttackLeft:
                    if (!m_AnimatorHelper.PrepareMeleeAttackL(m_IntegerParam))
                    {
                        m_Status = HiraBotsTaskResult.Failed;
                        return;
                    }
                    break;
                case MontageType.Unsheathe:
                    if (!m_AnimatorHelper.PrepareToEquip(m_WeaponTypeParam))
                    {
                        m_Status = HiraBotsTaskResult.Failed;
                        return;
                    }
                    break;
                case MontageType.Sheathe:
                    if (!m_AnimatorHelper.PrepareToUnequip(m_WeaponTypeParam))
                    {
                        m_Status = HiraBotsTaskResult.Failed;
                        return;
                    }
                    break;
                case MontageType.Bow:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            m_AnimatorHelper.currentMontageState = m_Type;
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            return m_Status;
        }

        public void Abort()
        {
            m_AnimatorHelper.currentMontageState = MontageType.None;
            Recycle();
        }

        public void End(bool success)
        {
            Recycle();
        }

        private void Recycle()
        {
            m_AnimatorHelper.stateExit.RemoveListener(OnAnimatorStateExit);
            m_AnimatorHelper = null;
            m_Type = MontageType.None;
            m_Status = HiraBotsTaskResult.InProgress;
            m_WeaponTypeParam = null;
            m_IntegerParam = null;
            s_Executables.Push(this);
        }

        private void OnAnimatorStateExit(MontageType type)
        {
            if (m_Type == type && m_Status == HiraBotsTaskResult.InProgress)
            {
                m_Status = HiraBotsTaskResult.Succeeded;
            }
        }

        public void OnMessageReceived(InterruptMontageMessage message)
        {
            if (m_Status == HiraBotsTaskResult.InProgress)
            {
                m_AnimatorHelper.currentMontageState = MontageType.None;
                m_Status = HiraBotsTaskResult.Failed;
            }
        }
    }

    public class PlayMontageTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private MontageType m_Type;
        [SerializeField] private bool m_UseCustomParam;
        [SerializeField] private int m_IntegerParam;
        [SerializeField] private EquipmentType m_EquipmentTypeParam;

        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            if (archetype is IHiraBotArchetype<AnimatorHelper> animated && animated.component != null)
            {
                return PlayMontageTask.Get(
                    animated.component,
                    m_Type,
                    m_UseCustomParam ? m_EquipmentTypeParam : null,
                    m_UseCustomParam ? m_IntegerParam : null);
            }

            return null;
        }
    }
}