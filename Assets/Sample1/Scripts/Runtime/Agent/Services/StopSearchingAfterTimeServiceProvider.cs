using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class StopSearchingAfterTimeService : IHiraBotsService
    {
        public float m_WaitTime;
        public BlackboardComponent m_Blackboard;
        public string m_CelebrityStatusKey;

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
                m_Blackboard.SetEnumValue<CelebrityStatus>(m_CelebrityStatusKey, CelebrityStatus.Unknown);
            }
        }

        public void Stop()
        {
            m_Blackboard = default;
        }
    }

    public class StopSearchingAfterTimeServiceProvider : HiraBotsServiceProvider
    {
        [SerializeField] private float m_WaitTime;
        [SerializeField] private BlackboardTemplate.KeySelector m_CelebrityStatus;

        #region Validation Boilerplate

        protected override void OnValidateCallback()
        {
            m_CelebrityStatus.keyTypesFilter = BlackboardKeyType.Enum;
        }

        protected override void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (!m_CelebrityStatus.Validate(in keySet, BlackboardKeyType.Enum))
            {
                reportError("no celebrity status key");
            }
        }

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            m_CelebrityStatus.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        #endregion

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return new StopSearchingAfterTimeService
            {
                m_Blackboard = blackboard,
                m_CelebrityStatusKey = m_CelebrityStatus.selectedKey.name,
                m_WaitTime = m_WaitTime
            };
        }
    }
}