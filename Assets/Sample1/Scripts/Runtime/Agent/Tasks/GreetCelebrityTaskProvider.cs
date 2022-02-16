using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public struct FanGreetingMessage
    {
        public GameObject m_Fan;
    }

    public class GreetCelebrityTask : IHiraBotsTask
    {
        public GameObject m_SelfGameObject;
        public BlackboardComponent m_Blackboard;
        public string m_CelebrityGameObjectKey;

        public void Begin()
        {
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            var obj = m_Blackboard.GetObjectValue(m_CelebrityGameObjectKey) as GameObject;
            if (obj == null)
            {
                return HiraBotsTaskResult.Failed;
            }

            if (!obj.TryGetComponent<BaseArchetype>(out var archetype)
                || archetype is not IHiraBotArchetype<HiraLGOAPRealtimeBot> bot
                || bot.component == null)
            {
                Debug.LogError("not supposed to happen");
                return HiraBotsTaskResult.Failed;
            }

            bot.component.Message(new FanGreetingMessage { m_Fan = m_SelfGameObject });
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

    public class GreetCelebrityTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private BlackboardTemplate.KeySelector m_CelebrityGameObject;

        #region Validation Boilerplate

        protected override void OnValidateCallback()
        {
            m_CelebrityGameObject.keyTypesFilter = BlackboardKeyType.Object;
        }

        protected override void Validate(System.Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (!m_CelebrityGameObject.Validate(in keySet, BlackboardKeyType.Object))
            {
                reportError("no celebrity game object key");
            }
        }

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            m_CelebrityGameObject.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        #endregion

        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return new GreetCelebrityTask
            {
                m_SelfGameObject = archetype.gameObject,
                m_Blackboard = blackboard,
                m_CelebrityGameObjectKey = m_CelebrityGameObject.selectedKey.name
            };
        }
    }
}