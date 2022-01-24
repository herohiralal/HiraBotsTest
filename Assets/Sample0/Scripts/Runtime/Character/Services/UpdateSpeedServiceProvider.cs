using UnityEngine;

namespace AIEngineTest.Services
{
    public class UpdateSpeedService : IHiraBotsService
    {
        public UpdateSpeedService(Transform transform, Animator animator, float min, float max)
        {
            m_Transform = transform;
            m_Animator = animator;
            m_Min = min;
            m_Max = max;
        }

        private Transform m_Transform;
        private Animator m_Animator;
        private readonly float m_Min, m_Max;

        private Vector3 m_Position;

        public void Start()
        {
            m_Position = m_Transform.position;
        }

        public void Tick(float deltaTime)
        {
            var newPos = m_Transform.position;
            var diff = newPos - m_Position;
            var velocity = diff / deltaTime;
            var speed = velocity.magnitude;

            var normalizedSpeed = (speed - m_Min) / (m_Max - m_Min);

            m_Position = newPos;

            m_Animator.SetFloat(AnimatorHashes.s_Speed, normalizedSpeed);
        }

        public void Stop()
        {
            m_Transform = null;
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
            if (archetype is not IHiraBotArchetype<Animator> animatedSoldier)
            {
                Debug.LogError("Attempted to get an update speed service for an invalid game object.");
                return null;
            }

            return new UpdateSpeedService(animatedSoldier.gameObject.transform, animatedSoldier.component, m_MinSpeed, m_MaxSpeed);
        }
    }
}