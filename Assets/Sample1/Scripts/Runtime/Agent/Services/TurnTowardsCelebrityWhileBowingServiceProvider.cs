using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    internal class TurnTowardsCelebrityWhileBowing : IHiraBotsService
    {
        public GameObject m_SelfGameObject;
        public BlackboardComponent m_Blackboard;
        public string m_CelebrityGameObjectKey;
        public AnimatorHelper m_Animator;

        private bool m_On = false;

        public void Start()
        {
            m_Animator.stateEnter.AddListener(OnStateEnter);
            m_Animator.stateExit.AddListener(OnStateExit);
        }

        public void Stop()
        {
            m_Animator.stateExit.RemoveListener(OnStateExit);
            m_Animator.stateEnter.RemoveListener(OnStateEnter);
        }

        public void Tick(float deltaTime)
        {
            if (!m_On)
            {
                return;
            }

            var obj = m_Blackboard.GetObjectValue(m_CelebrityGameObjectKey) as GameObject;
            if (obj == null)
            {
                return;
            }

            var transform = m_SelfGameObject.transform;
            var eulerAngles = transform.eulerAngles;
            transform.LookAt(obj.transform);
            eulerAngles.y = transform.eulerAngles.y; // only need to change the yaw
            transform.eulerAngles = eulerAngles;
        }

        private void OnStateEnter(MontageType arg0)
        {
            if (arg0 == MontageType.Bow)
            {
                m_On = true;
            }
        }

        private void OnStateExit(MontageType arg0)
        {
            if (arg0 == MontageType.Bow)
            {
                m_On = false;
            }
        }
    }

    internal class TurnTowardsCelebrityWhileBowingServiceProvider : HiraBotsServiceProvider
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

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            if (archetype is IHiraBotArchetype<AnimatorHelper> animator)
            {
                return new TurnTowardsCelebrityWhileBowing
                {
                    m_SelfGameObject = archetype.gameObject,
                    m_Blackboard = blackboard,
                    m_CelebrityGameObjectKey = m_CelebrityGameObject.selectedKey.name,
                    m_Animator = animator.component
                };
            }

            return null;
        }
    }
}