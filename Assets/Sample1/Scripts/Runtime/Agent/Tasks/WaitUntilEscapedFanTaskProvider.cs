using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    internal class EscapeFanTask : IHiraBotsTask
    {
        public GameObject m_SelfGameObject;
        public float m_DistanceSq;

        public BlackboardComponent m_Blackboard;
        public string m_CreepedOutByKey;

        public float m_WaitTime;

        public void Begin()
        {
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            if (m_SelfGameObject == null)
            {
                return HiraBotsTaskResult.Failed;
            }

            var creepedOutBy = m_Blackboard.GetObjectValue(m_CreepedOutByKey) as GameObject;
            if (creepedOutBy == null)
            {
                m_Blackboard.SetObjectValue(m_CreepedOutByKey, null);
                return HiraBotsTaskResult.Succeeded;
            }

            var distSq = (m_SelfGameObject.transform.position - creepedOutBy.transform.position).sqrMagnitude;
            if (distSq < m_DistanceSq)
            {
                return HiraBotsTaskResult.Failed;
            }

            m_WaitTime -= deltaTime;
            if (m_WaitTime > 0f)
            {
                return HiraBotsTaskResult.InProgress;
            }

            return HiraBotsTaskResult.Succeeded;
        }

        public void Abort()
        {
            m_SelfGameObject = null;
            m_Blackboard = default;
        }

        public void End(bool success)
        {
            m_SelfGameObject = null;
            m_Blackboard = default;
        }
    }

    internal class WaitUntilEscapedFanTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private BlackboardTemplate.KeySelector m_CreepedOutBy;
        [SerializeField] private float m_Distance = 5f;
        [SerializeField] private float m_WaitFor = 5f;

        #region Validation Boilerplate

        protected override void OnValidateCallback()
        {
            m_CreepedOutBy.keyTypesFilter = BlackboardKeyType.Object;
        }

        protected override void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (!m_CreepedOutBy.Validate(keySet, BlackboardKeyType.Object))
            {
                reportError("no creeped out by key");
            }
        }

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            m_CreepedOutBy.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        #endregion

        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return new EscapeFanTask
            {
                m_Blackboard = blackboard,
                m_CreepedOutByKey = m_CreepedOutBy.selectedKey.name,
                m_DistanceSq = m_Distance * m_Distance,
                m_SelfGameObject = archetype.gameObject,
                m_WaitTime = m_WaitFor
            };
        }
    }
}