using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public struct InterruptMontageMessage
    {
        public bool m_Success;
    }

    public struct FailMontageWithoutInterruptMessage
    {
    }

    public class PlayMontageTask : IHiraBotsTask, IMessageListener<InterruptMontageMessage>, IMessageListener<FailMontageWithoutInterruptMessage>
    {
        private PlayMontageTask()
        {
        }

        private enum AnimationStatus
        {
            NotStarted,
            Ongoing,
            Interrupted,
            Completed
        }

        private AnimatorHelper m_AnimatorHelper;
        private MontageType m_Type;
        private int? m_IntegerParam;
        private AnimationStatus m_AnimationStatus;
        private bool m_SucceedOnAnimationCompletion;
        private float? m_Duration;

        public static PlayMontageTask Get(AnimatorHelper animatorHelper, MontageType type, int? integerParam, float duration)
        {
            var output = s_Executables.Count == 0 ? new PlayMontageTask() : s_Executables.Pop();
            output.m_AnimatorHelper = animatorHelper;
            output.m_Type = type;
            output.m_AnimationStatus = AnimationStatus.NotStarted;
            output.m_SucceedOnAnimationCompletion = true;
            output.m_IntegerParam = integerParam;
            output.m_Duration = duration <= 0f ? null : duration;
            return output;
        }

        private static readonly Stack<PlayMontageTask> s_Executables = new Stack<PlayMontageTask>();

        public void Begin()
        {
            m_AnimatorHelper.stateEnter.AddListener(OnAnimatorStateEnter);
            m_AnimatorHelper.stateExit.AddListener(OnAnimatorStateExit);

            switch (m_Type)
            {
                case MontageType.None:
                    m_AnimationStatus = AnimationStatus.Completed;
                    return; // no need to run the montage
                case MontageType.MeleeAttackRight:
                    if (!m_AnimatorHelper.PrepareMeleeAttackR(m_IntegerParam))
                    {
                        m_AnimationStatus = AnimationStatus.Completed;
                        return;
                    }
                    break;
                case MontageType.MeleeAttackLeft:
                    if (!m_AnimatorHelper.PrepareMeleeAttackL(m_IntegerParam))
                    {
                        m_AnimationStatus = AnimationStatus.Completed;
                        return;
                    }
                    break;
                case MontageType.Unsheathe:
                    if (!m_AnimatorHelper.PrepareToEquip())
                    {
                        m_AnimationStatus = AnimationStatus.Completed;
                        return;
                    }
                    break;
                case MontageType.Sheathe:
                    if (!m_AnimatorHelper.PrepareToUnequip())
                    {
                        m_AnimationStatus = AnimationStatus.Completed;
                        return;
                    }
                    break;
                case MontageType.Bow:
                    break;
                case MontageType.Die:
                    break;
                case MontageType.Hit:
                    break;
                case MontageType.Block:
                    break;
                case MontageType.Dodge:
                    break;
                case MontageType.Cast:
                    break;
                case MontageType.DualAttack:
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            m_AnimatorHelper.currentMontageState = m_Type;
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            switch (m_AnimationStatus)
            {
                case AnimationStatus.NotStarted:
                {
                    return HiraBotsTaskResult.InProgress;
                }
                case AnimationStatus.Ongoing:
                {
                    UpdateDuration(deltaTime);
                    m_Duration -= deltaTime;
                    return HiraBotsTaskResult.InProgress;
                }
                case AnimationStatus.Interrupted:
                {
                    return HiraBotsTaskResult.Failed;
                }
                case AnimationStatus.Completed:
                {
                    UpdateDuration(deltaTime);
                    if (m_Duration.HasValue)
                    {
                        return HiraBotsTaskResult.InProgress;
                    }

                    return m_SucceedOnAnimationCompletion ? HiraBotsTaskResult.Succeeded : HiraBotsTaskResult.Failed;
                }
                default:
                {
                    throw new System.ArgumentOutOfRangeException();
                }
            }
        }

        private void UpdateDuration(float deltaTime)
        {
            if (!m_Duration.HasValue) return;

            m_Duration = m_Duration.Value - deltaTime;
            if (m_Duration.Value > 0f) return;

            m_AnimatorHelper.keepMontageActive = false;
            m_Duration = null;
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
            m_AnimatorHelper.keepMontageActive = false;
            m_AnimatorHelper.stateEnter.RemoveListener(OnAnimatorStateEnter);
            m_AnimatorHelper.stateExit.RemoveListener(OnAnimatorStateExit);
            m_AnimatorHelper = null;
            s_Executables.Push(this);
        }

        private void OnAnimatorStateEnter(MontageType type)
        {
            if (m_Type == type && m_AnimationStatus == AnimationStatus.NotStarted)
            {
                m_AnimationStatus = AnimationStatus.Ongoing;

                if (m_Duration.HasValue)
                {
                    m_AnimatorHelper.keepMontageActive = true;
                }
            }
        }

        private void OnAnimatorStateExit(MontageType type)
        {
            if (m_Type == type && m_AnimationStatus == AnimationStatus.Ongoing)
            {
                m_AnimationStatus = AnimationStatus.Completed;
            }
        }

        public void OnMessageReceived(InterruptMontageMessage message)
        {
            if (m_AnimationStatus != AnimationStatus.Interrupted)
            {
                m_AnimationStatus = AnimationStatus.Interrupted;
                m_SucceedOnAnimationCompletion = message.m_Success && m_SucceedOnAnimationCompletion;
                m_AnimatorHelper.currentMontageState = MontageType.None;
            }
        }

        public void OnMessageReceived(FailMontageWithoutInterruptMessage message)
        {
            m_SucceedOnAnimationCompletion = false;
        }
    }

    public class PlayMontageTaskProvider : HiraBotsTaskProvider
    {
        [System.Serializable]
        public struct MontageInfo
        {
            public MontageType m_Type;
            public bool m_UseExtraParam;
            public int m_ExtraParam;
            public float m_Duration;
        }

        [SerializeField] private MontageInfo m_Info = new MontageInfo
        {
            m_Type = MontageType.None,
            m_UseExtraParam = false,
            m_ExtraParam = 1,
            m_Duration = -1f
        };

        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            if (archetype is IHiraBotArchetype<AnimatorHelper> animated && animated.component != null)
            {
                return PlayMontageTask.Get(
                    animated.component,
                    m_Info.m_Type,
                    m_Info.m_UseExtraParam ? m_Info.m_ExtraParam : null,
                    m_Info.m_Duration);
            }

            return null;
        }
    }
}