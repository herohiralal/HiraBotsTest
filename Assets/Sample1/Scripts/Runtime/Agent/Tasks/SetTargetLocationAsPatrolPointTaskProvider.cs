using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    internal class SetTargetLocationAsPatrolPointTask : IHiraBotsTask
    {
        public BlackboardComponent m_Blackboard;
        public MarkupPointCollection m_PatrolPointCollection;
        public string m_Key;

        public void Begin()
        {
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            Vector3 destination;
            Vector3 current = m_Blackboard.GetVectorValue(m_Key);
            do
            {
                destination = m_PatrolPointCollection.GetRandom();
            } while (destination == current);

            m_Blackboard.SetVectorValue(m_Key, destination, true);

            return HiraBotsTaskResult.Succeeded;
        }

        public void Abort()
        {
        }

        public void End(bool success)
        {
        }
    }

    internal class SetTargetLocationAsPatrolPointTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private BlackboardTemplate.KeySelector m_Key;
        [SerializeField] private MarkupPointCollection m_PatrolPointCollection;

        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return new SetTargetLocationAsPatrolPointTask
            {
                m_Blackboard = blackboard,
                m_PatrolPointCollection = m_PatrolPointCollection,
                m_Key = m_Key.selectedKey.name
            };
        }

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            m_Key.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        protected override void OnValidateCallback()
        {
            m_Key.keyTypesFilter = BlackboardKeyType.Vector;
        }

        protected override void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (!m_Key.Validate(in keySet, BlackboardKeyType.Vector))
            {
                reportError("Unassigned key.");
            }

            if (m_PatrolPointCollection == null)
            {
                reportError("No patrol point collection present.");
            }
        }
    }
}