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
        private HiraBotsTaskResult m_Status;

        public static PlayMontageTask Get(AnimatorHelper animatorHelper, MontageType type)
        {
            var output = s_Executables.Count == 0 ? new PlayMontageTask() : s_Executables.Pop();
            output.m_AnimatorHelper = animatorHelper;
            output.m_Type = type;
            output.m_Status = HiraBotsTaskResult.InProgress;
            return output;
        }

        private static readonly Stack<PlayMontageTask> s_Executables = new Stack<PlayMontageTask>();

        public void Begin()
        {
            m_AnimatorHelper.stateExit.AddListener(OnAnimatorStateExit);
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

        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            if (archetype is IHiraBotArchetype<AnimatorHelper> animated && animated.component != null)
            {
                return PlayMontageTask.Get(animated.component, m_Type);
            }

            return null;
        }
    }
}