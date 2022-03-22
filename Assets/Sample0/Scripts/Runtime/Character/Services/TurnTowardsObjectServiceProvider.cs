using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class TurnTowardsTransformService : IHiraBotsService
    {
        public static TurnTowardsTransformService Get(Transform self, Transform other)
        {
            var output = s_Executables.Count > 0 ? s_Executables.Pop() : new TurnTowardsTransformService();
            output.m_Self = self;
            output.m_Other = other;
            return output;
        }

        private TurnTowardsTransformService()
        {
        }

        private Transform m_Self = null;
        private Transform m_Other = null;

        private static readonly Stack<TurnTowardsTransformService> s_Executables = new Stack<TurnTowardsTransformService>();

        public void Start()
        {
            // no op
        }

        public void Tick(float deltaTime)
        {
            var direction = m_Other.position - m_Self.position;
            direction.y = 0; // ignore y component
            direction.Normalize();
            m_Self.forward = direction;
        }

        public void Stop()
        {
            s_Executables.Push(this);
        }
    }

    public class TurnTowardsObjectServiceProvider : HiraBotsServiceProvider
    {
        [SerializeField] private BlackboardTemplate.KeySelector m_Target;

        #region Validation Boilerplate

        protected override void OnValidateCallback()
        {
            m_Target.keyTypesFilter = BlackboardKeyType.Object;
        }

        protected override void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (!m_Target.Validate(in keySet, BlackboardKeyType.Object))
            {
                reportError("no target game object key");
            }
        }

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            m_Target.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        #endregion

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype arch)
        {
            var obj = blackboard.GetObjectValue(m_Target.selectedKey.name);
            return obj is Component other
                ? TurnTowardsTransformService.Get(arch.gameObject.transform, other.transform)
                : null;
        }
    }
}