using System.Collections.Generic;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class GetUpFromRagdollTask : IHiraBotsTask
    {
        public static GetUpFromRagdollTask Get(AnimatorHelper animatorHelper, BlackboardComponent blackboard)
        {
            var output = s_Executables.Count > 0 ? s_Executables.Pop() : new GetUpFromRagdollTask();
            output.m_AnimatorHelper = animatorHelper;
            output.m_Blackboard = blackboard;
            output.m_Finished = false;
            return output;
        }

        private BlackboardComponent m_Blackboard;
        private AnimatorHelper m_AnimatorHelper;
        private bool m_Finished;

        private static readonly Stack<GetUpFromRagdollTask> s_Executables = new Stack<GetUpFromRagdollTask>();

        private GetUpFromRagdollTask()
        {
        }

        public void Begin()
        {
            m_AnimatorHelper.TriggerRagdollOff();
            m_Blackboard.SetBooleanValue("Ragdoll", false, true);
            m_AnimatorHelper.getUpFromRagdoll.AddListener(GetUpFromRagdoll);
        }

        private void GetUpFromRagdoll()
        {
            m_Finished = true;
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            return m_Finished ? HiraBotsTaskResult.Succeeded : HiraBotsTaskResult.InProgress;
        }

        public void Abort()
        {
            Recycle();
        }

        public void End(bool success)
        {
            Recycle();
        }

        private void Recycle()
        {
            m_AnimatorHelper.getUpFromRagdoll.RemoveListener(GetUpFromRagdoll);
            m_Blackboard = default;
            m_AnimatorHelper = null;
            m_Finished = false;
            s_Executables.Push(this);
        }
    }

    public class GetUpFromRagdollTaskProvider : HiraBotsTaskProvider
    {
        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return archetype is IHiraBotArchetype<AnimatorHelper> animated
                ? GetUpFromRagdollTask.Get(animated.component, blackboard)
                : null;
        }
    }
}