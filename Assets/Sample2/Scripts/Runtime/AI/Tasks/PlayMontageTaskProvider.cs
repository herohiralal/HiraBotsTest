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

        private Sample2Animator m_Animator;
        private Sample2MontageType m_Type;
        private HiraBotsTaskResult m_Status;

        public static PlayMontageTask Get(Sample2Animator animator, Sample2MontageType type)
        {
            var output = s_Executables.Count == 0 ? new PlayMontageTask() : s_Executables.Pop();
            output.m_Animator = animator;
            output.m_Type = type;
            output.m_Status = HiraBotsTaskResult.InProgress;
            return output;
        }

        private static readonly Stack<PlayMontageTask> s_Executables = new Stack<PlayMontageTask>();

        public void Begin()
        {
            m_Animator.stateExit.AddListener(OnAnimatorStateExit);
            m_Animator.currentMontageState = m_Type;
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            return m_Status;
        }

        public void Abort()
        {
            m_Animator.currentMontageState = Sample2MontageType.None;
            Recycle();
        }

        public void End(bool success)
        {
            Recycle();
        }

        private void Recycle()
        {
            m_Animator.stateExit.RemoveListener(OnAnimatorStateExit);
            m_Animator = null;
            m_Type = Sample2MontageType.None;
            m_Status = HiraBotsTaskResult.InProgress;
            s_Executables.Push(this);
        }

        private void OnAnimatorStateExit(Sample2MontageType type)
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
                m_Animator.currentMontageState = Sample2MontageType.None;
                m_Status = HiraBotsTaskResult.Failed;
            }
        }
    }

    public class PlayMontageTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private Sample2MontageType m_Type;

        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            if (archetype is IHiraBotArchetype<Sample2Animator> animated && animated.component != null)
            {
                return PlayMontageTask.Get(animated.component, m_Type);
            }

            return null;
        }
    }
}