using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace AIEngineTest
{
    public class LookOutForCelebrityService : IHiraBotsService
    {
        private bool m_Created = false;
        public BlackboardComponent m_Blackboard;
        public string m_CelebrityStatusKey;
        public string m_CelebrityGameObjectKey;
        public string m_CelebrityTag;
        public ConsolidatedSensor m_Sensor;

        public void Start()
        {
            m_Sensor.newObjectPerceived.AddListener(OnObjectFound);

            m_Created = true;
        }

        public void Tick(float deltaTime)
        {
        }

        public void Stop()
        {
            m_Created = false;

            m_Sensor.newObjectPerceived.RemoveListener(OnObjectFound);

            m_Blackboard = default;
            m_Sensor = null;
        }

        private void OnObjectFound(Object o)
        {
            if (!m_Created)
            {
                Debug.LogError("This shouldn't have happened.");
                return;
            }

            if (o != null && o is GameObject go && go.CompareTag(m_CelebrityTag))
            {
                m_Blackboard.SetEnumValue<CelebrityStatus>(m_CelebrityStatusKey, CelebrityStatus.Known);
                m_Blackboard.SetObjectValue(m_CelebrityGameObjectKey, go);
            }
        }
    }

    public class LookOutForCelebrityServiceProvider : HiraBotsServiceProvider
    {
        public LookOutForCelebrityServiceProvider()
        {
            tickInterval = 1000f;
        }

        [SerializeField, TagProperty] private string m_CelebrityTag;
        [SerializeField] private BlackboardTemplate.KeySelector m_CelebrityStatus;
        [SerializeField] private BlackboardTemplate.KeySelector m_CelebrityGameObject;

        #region Validation Boilerplate

        protected override void OnValidateCallback()
        {
            m_CelebrityStatus.keyTypesFilter = BlackboardKeyType.Enum;
            m_CelebrityGameObject.keyTypesFilter = BlackboardKeyType.Object;
        }

        protected override void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (!m_CelebrityStatus.Validate(in keySet, BlackboardKeyType.Enum))
            {
                reportError("no celebrity status key");
            }

            if (!m_CelebrityGameObject.Validate(in keySet, BlackboardKeyType.Object))
            {
                reportError("no celebrity game object key");
            }
        }

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            m_CelebrityStatus.OnTargetBlackboardTemplateChanged(template, in keySet);
            m_CelebrityGameObject.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        #endregion

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            if (archetype is IHiraBotArchetype<ConsolidatedSensor> sensor)
            {
                return new LookOutForCelebrityService
                {
                    m_Blackboard = blackboard,
                    m_CelebrityStatusKey = m_CelebrityStatus.selectedKey.name,
                    m_CelebrityGameObjectKey = m_CelebrityGameObject.selectedKey.name,
                    m_CelebrityTag = m_CelebrityTag,
                    m_Sensor = sensor.component
                };
            }

            return null;
        }
    }
}