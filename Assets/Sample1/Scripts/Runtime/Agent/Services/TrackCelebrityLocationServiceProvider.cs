using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class TrackCelebrityLocationService : IHiraBotsService
    {
        private bool m_Created = false;
        private bool m_DoNotTick;
        public BlackboardComponent m_Blackboard;
        public string m_CelebrityStatusKey;
        public string m_CelebrityGameObjectKey;
        public string m_CelebrityLocationKey;
        public string m_CelebrityTag;
        public ConsolidatedSensor m_Sensor;

        public void Start()
        {
            m_Sensor.objectStoppedPerceiving.AddListener(OnObjectLost);

            m_DoNotTick = false;
            m_Created = true;
        }

        public void Tick(float deltaTime)
        {
            if (m_DoNotTick)
            {
                return;
            }

            var go = m_Blackboard.GetObjectValue(m_CelebrityGameObjectKey) as GameObject;
            if (go != null)
            {
                m_Blackboard.SetVectorValue(m_CelebrityLocationKey, go.transform.position);
            }
            else
            {
                Lost();
            }
        }

        public void Stop()
        {
            m_Created = false;

            m_Sensor.objectStoppedPerceiving.RemoveListener(OnObjectLost);
            m_Blackboard = default;
            m_Sensor = null;
        }

        private void OnObjectLost(Object o)
        {
            if (!m_Created)
            {
                Debug.LogError("This shouldn't have happened.");
                return;
            }

            if (o is GameObject go && go.CompareTag(m_CelebrityTag))
            {
                Lost();
            }
        }

        private void Lost()
        {
            m_Blackboard.SetObjectValue(m_CelebrityGameObjectKey, null);
            m_Blackboard.SetEnumValue<CelebrityStatus>(m_CelebrityStatusKey, CelebrityStatus.Lost);
            m_DoNotTick = true;
        }
    }

    public class TrackCelebrityLocationServiceProvider : HiraBotsServiceProvider
    {
        [SerializeField, TagProperty] private string m_CelebrityTag;
        [SerializeField] private BlackboardTemplate.KeySelector m_CelebrityStatus;
        [SerializeField] private BlackboardTemplate.KeySelector m_CelebrityGameObject;
        [SerializeField] private BlackboardTemplate.KeySelector m_CelebrityLocation;

        #region Validation Boilerplate

        protected override void OnValidateCallback()
        {
            m_CelebrityStatus.keyTypesFilter = BlackboardKeyType.Enum;
            m_CelebrityGameObject.keyTypesFilter = BlackboardKeyType.Object;
            m_CelebrityLocation.keyTypesFilter = BlackboardKeyType.Vector;
        }

        protected override void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (!m_CelebrityStatus.Validate(in keySet, BlackboardKeyType.Enum))
            {
                reportError("no celebrity status key");
            }

            if (!m_CelebrityGameObject.Validate(in keySet, BlackboardKeyType.Object))
            {
                reportError("no celebrity tracked transform key");
            }

            if (!m_CelebrityLocation.Validate(in keySet, BlackboardKeyType.Vector))
            {
                reportError("no celebrity tracked transform key");
            }
        }

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            m_CelebrityStatus.OnTargetBlackboardTemplateChanged(template, in keySet);
            m_CelebrityGameObject.OnTargetBlackboardTemplateChanged(template, in keySet);
            m_CelebrityLocation.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        #endregion

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            if (archetype is IHiraBotArchetype<ConsolidatedSensor> sensor)
            {
                return new TrackCelebrityLocationService
                {
                    m_Blackboard = blackboard,
                    m_CelebrityStatusKey = m_CelebrityStatus.selectedKey.name,
                    m_CelebrityGameObjectKey = m_CelebrityGameObject.selectedKey.name,
                    m_CelebrityLocationKey = m_CelebrityLocation.selectedKey.name,
                    m_CelebrityTag = m_CelebrityTag,
                    m_Sensor = sensor.component
                };
            }

            return null;
        }
    }
}