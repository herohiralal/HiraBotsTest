using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public struct FanGreetingMessage
    {
        public GameObject m_Fan;
    }

    public class BowToCelebrityTask : IHiraBotsTask
    {
        public Animator m_Animator;
        public float m_AnimationTime;

        public GameObject m_SelfGameObject;

        public BlackboardComponent m_Blackboard;
        public string m_CelebrityGameObjectKey;

        public void Begin()
        {
            m_Animator.SetTrigger(AnimatorHashes.s_Bowing);
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            m_AnimationTime -= deltaTime;
            if (m_AnimationTime > 0f)
            {
                return HiraBotsTaskResult.InProgress;
            }

            var obj = m_Blackboard.GetObjectValue(m_CelebrityGameObjectKey) as GameObject;
            if (obj == null
                || !obj.TryGetComponent<Archetype>(out var archetype)
                || archetype is not IHiraBotArchetype<HiraLGOAPRealtimeBot> bot)
            {
                return HiraBotsTaskResult.Failed;
            }

            bot.component.Message(new FanGreetingMessage { m_Fan = m_SelfGameObject });
            return HiraBotsTaskResult.Succeeded;
        }

        public void Abort()
        {
            m_SelfGameObject = null;
            m_Animator = null;
            m_Blackboard = default;
        }

        public void End(bool success)
        {
            m_SelfGameObject = null;
            m_Animator = null;
            m_Blackboard = default;
        }
    }

    public class BowToCelebrityTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private float m_AnimationTime = 3f;
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
            if (archetype is IHiraBotArchetype<Animator> animator)
            {
                return new BowToCelebrityTask
                {
                    m_Animator = animator.component,
                    m_AnimationTime = m_AnimationTime,
                    m_SelfGameObject = archetype.gameObject,
                    m_Blackboard = blackboard,
                    m_CelebrityGameObjectKey = m_CelebrityGameObject.selectedKey.name
                };
            }

            return null;
        }
    }
}