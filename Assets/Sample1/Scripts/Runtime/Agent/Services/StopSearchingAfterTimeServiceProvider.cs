using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    internal class StopSearchingAfterTimeService : IHiraBotsService
    {
        public float m_WaitTime;
        public BlackboardComponent m_Blackboard;
        public string m_ExcitementKey;

        public void Start()
        {
        }

        public void Tick(float deltaTime)
        {
            if (m_WaitTime < 0f)
            {
                return;
            }

            m_WaitTime -= deltaTime;

            if (m_WaitTime < 0f)
            {
                m_Blackboard.SetFloatValue(m_ExcitementKey, 0f);
            }
        }

        public void Stop()
        {
            m_Blackboard = default;
        }
    }

    internal class StopSearchingAfterTimeServiceProvider : HiraBotsServiceProvider
    {
        [SerializeField] private float m_WaitTime;
        [SerializeField] private BlackboardTemplate.KeySelector m_Excitement;

        #region Validation Boilerplate

        protected override void OnValidateCallback()
        {
            m_Excitement.keyTypesFilter = BlackboardKeyType.Float;
        }

        protected override void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (!m_Excitement.Validate(in keySet, BlackboardKeyType.Float))
            {
                reportError("no celebrity status key");
            }
        }

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            m_Excitement.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        #endregion

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return new StopSearchingAfterTimeService
            {
                m_Blackboard = blackboard,
                m_ExcitementKey = m_Excitement.selectedKey.name,
                m_WaitTime = m_WaitTime
            };
        }
    }
}