using System.Collections.Generic;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class StartRagdollTask : IHiraBotsTask
    {
        public static StartRagdollTask Get(AnimatorHelper animatorHelper, BlackboardComponent blackboard)
        {
            var output = s_Executables.Count > 0 ? s_Executables.Pop() : new StartRagdollTask();
            output.m_AnimatorHelper = animatorHelper;
            output.m_Blackboard = blackboard;
            return output;
        }

        private BlackboardComponent m_Blackboard;
        private AnimatorHelper m_AnimatorHelper;

        private static readonly Stack<StartRagdollTask> s_Executables = new Stack<StartRagdollTask>();

        private StartRagdollTask()
        {
        }

        public void Begin()
        {
            m_AnimatorHelper.TriggerRagdollOn();
            m_Blackboard.SetBooleanValue("Ragdoll", true, true);
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            return HiraBotsTaskResult.InProgress;
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
            m_Blackboard = default;
            m_AnimatorHelper = null;
            s_Executables.Push(this);
        }
    }

    public class StartRagdollTaskProvider : HiraBotsTaskProvider
    {
        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return archetype is IHiraBotArchetype<AnimatorHelper> animated
                ? StartRagdollTask.Get(animated.component, blackboard)
                : null;
        }
    }
}