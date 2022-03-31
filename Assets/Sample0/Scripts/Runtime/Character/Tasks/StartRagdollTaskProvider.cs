using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class StartRagdollTask : IHiraBotsTask
    {
        public static StartRagdollTask Get(AnimatorHelper animatorHelper, BlackboardComponent blackboard, LayerMask groundLayer)
        {
            var output = s_Executables.Count > 0 ? s_Executables.Pop() : new StartRagdollTask();
            output.m_AnimatorHelper = animatorHelper;
            output.m_Blackboard = blackboard;
            output.m_GroundLayer = groundLayer;
            return output;
        }

        private BlackboardComponent m_Blackboard;
        private AnimatorHelper m_AnimatorHelper;
        private LayerMask m_GroundLayer;

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
            const float maxHipHeight = 0.8762761f;

            var rootTransform = m_AnimatorHelper.transform;

            var hipTransform = m_AnimatorHelper.ragdollRoot.transform;

            var originalParent = hipTransform.parent;
            hipTransform.SetParent(null);

            var hipPosition = hipTransform.position;

            var hipHeight = Physics.Raycast(hipPosition, Vector3.down, out var hit, maxHipHeight, m_GroundLayer)
                ? hit.distance
                : maxHipHeight;

            rootTransform.position = hipPosition - new Vector3(0, hipHeight, 0);

            var hipPivot = m_AnimatorHelper.hipPivot;
            var neckPivot = m_AnimatorHelper.neckPivot;

            var rootForward = (Vector3.Dot(hipPivot.forward, Vector3.up) >= 0
                                  ? -1f
                                  : 1f)
                              * (neckPivot.position - hipPivot.position);

            rootForward.y = 0f;
            rootForward.Normalize();
            rootTransform.forward = rootForward;

            hipTransform.SetParent(originalParent);

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
        [SerializeField] private LayerMask m_GroundLayer = ~0;

        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return archetype is IHiraBotArchetype<AnimatorHelper> animated
                ? StartRagdollTask.Get(animated.component, blackboard, m_GroundLayer)
                : null;
        }
    }
}