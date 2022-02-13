using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class TrackCelebrityLocationService : IHiraBotsService
    {
        private bool m_Bound = false;
        private bool m_DoNotTick;
        public BlackboardComponent m_Blackboard;
        public string m_CelebrityGameObjectKey;
        public string m_CelebrityLocationKey;
        public string m_CelebrityTag;
        public ConsolidatedSensor m_Sensor;

        public void Start()
        {
            var found = false;
            foreach (var o in m_Sensor.perceivedObjects)
            {
                if (o != null && o is GameObject go && go.CompareTag(m_CelebrityTag))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Lost();
                return;
            }

            m_Sensor.objectStoppedPerceiving.AddListener(OnObjectLost);

            m_DoNotTick = false;
            m_Bound = true;
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
            if (m_Bound)
            {
                m_Sensor.objectStoppedPerceiving.RemoveListener(OnObjectLost);
            }

            m_Bound = false;

            m_Blackboard = default;
            m_Sensor = null;
        }

        private void OnObjectLost(Object o)
        {
            if (!m_Bound)
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
            m_DoNotTick = true;
        }
    }

    public class TrackCelebrityLocationServiceProvider : HiraBotsServiceProvider
    {
        [SerializeField, TagProperty] private string m_CelebrityTag;
        [SerializeField] private BlackboardTemplate.KeySelector m_CelebrityGameObject;
        [SerializeField] private BlackboardTemplate.KeySelector m_CelebrityLocation;

        #region Validation Boilerplate

        protected override void OnValidateCallback()
        {
            m_CelebrityGameObject.keyTypesFilter = BlackboardKeyType.Object;
            m_CelebrityLocation.keyTypesFilter = BlackboardKeyType.Vector;
        }

        protected override void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (!m_CelebrityGameObject.Validate(in keySet, BlackboardKeyType.Object))
            {
                reportError("no celebrity game object key");
            }

            if (!m_CelebrityLocation.Validate(in keySet, BlackboardKeyType.Vector))
            {
                reportError("no celebrity location");
            }
        }

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
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