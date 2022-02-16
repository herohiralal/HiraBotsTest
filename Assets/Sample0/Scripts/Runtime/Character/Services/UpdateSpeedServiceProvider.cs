using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class UpdateSpeedService : IHiraBotsService
    {
        public static UpdateSpeedService Get(NavMeshAgent agent, AnimatorHelper animator)
        {
            var output = s_Executables.Count > 0 ? s_Executables.Pop() : new UpdateSpeedService();
            output.m_NavMeshAgent = agent;
            output.m_Animator = animator;
            return output;
        }

        private UpdateSpeedService()
        {
        }

        private NavMeshAgent m_NavMeshAgent;
        private AnimatorHelper m_Animator;

        private static readonly Stack<UpdateSpeedService> s_Executables = new Stack<UpdateSpeedService>();

        public void Start()
        {
        }

        public void Tick(float deltaTime)
        {
            m_Animator.speed = m_NavMeshAgent.velocity.magnitude;
        }

        public void Stop()
        {
            m_NavMeshAgent = null;
            m_Animator = null;
            s_Executables.Push(this);
        }
    }

    public class UpdateSpeedServiceProvider : HiraBotsServiceProvider
    {
        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            if (archetype is not (IHiraBotArchetype<NavMeshAgent> navigatingSoldier and IHiraBotArchetype<AnimatorHelper> animated))
            {
                Debug.LogError("Attempted to get an update speed service for an invalid game object.");
                return null;
            }

            return UpdateSpeedService.Get(navigatingSoldier.component, animated.component);
        }
    }
}