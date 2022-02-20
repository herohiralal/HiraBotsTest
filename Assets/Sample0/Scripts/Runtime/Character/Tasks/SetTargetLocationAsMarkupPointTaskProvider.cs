using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace AIEngineTest
{
    public class SetTargetLocationAsMarkupPointTask : IHiraBotsTask
    {
        public BlackboardComponent m_Blackboard;
        public MarkupPointCollection m_MarkupPointCollection;
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
                destination = m_MarkupPointCollection.GetRandom();
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

    public class SetTargetLocationAsMarkupPointTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private BlackboardTemplate.KeySelector m_Key;
        [FormerlySerializedAs("m_PatrolPointCollection")]
        [SerializeField] private MarkupPointCollection m_MarkupPointCollection;

        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return new SetTargetLocationAsMarkupPointTask
            {
                m_Blackboard = blackboard,
                m_MarkupPointCollection = m_MarkupPointCollection,
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

            if (m_MarkupPointCollection == null)
            {
                reportError("No patrol point collection present.");
            }
        }
    }
}