using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class UpdateSpeedService : IHiraBotsService
    {
        public UpdateSpeedService(NavMeshAgent agent, Animator animator, float min, float max)
        {
            m_NavMeshAgent = agent;
            m_Animator = animator;
            m_Min = min;
            m_Max = max;
        }

        private NavMeshAgent m_NavMeshAgent;
        private Animator m_Animator;
        private readonly float m_Min, m_Max;

        public void Start()
        {
        }

        public void Tick(float deltaTime)
        {
            var speed = m_NavMeshAgent.velocity.magnitude;

            var normalizedSpeed = (speed - m_Min) / (m_Max - m_Min);

            m_Animator.SetFloat(AnimatorHashes.s_Speed, normalizedSpeed);
        }

        public void Stop()
        {
            m_NavMeshAgent = null;
            m_Animator = null;
        }
    }

    public class UpdateSpeedServiceProvider : HiraBotsServiceProvider
    {
        public UpdateSpeedServiceProvider()
        {
            tickInterval = 1f;
        }

        [SerializeField] private float m_MinSpeed = 0f, m_MaxSpeed = 3f;

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            if (archetype is not (IHiraBotArchetype<NavMeshAgent> navigatingSoldier and IHiraBotArchetype<Animator> animatedSoldier))
            {
                Debug.LogError("Attempted to get an update speed service for an invalid game object.");
                return null;
            }

            return new UpdateSpeedService(navigatingSoldier.component, animatedSoldier.component, m_MinSpeed, m_MaxSpeed);
        }
    }
}