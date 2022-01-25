using System;
using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class PatrolTask : IHiraBotsTask
    {
        public NavMeshAgent m_NavMeshAgent;
        public PatrolPointCollection m_PatrolPointCollection;
        public float m_MoveSpeed;
        public float m_StopDistance;
        public float m_WaitTime;

        private bool m_Waiting;
        private float m_TimeSpentWaiting;

        public void Begin()
        {
            m_Waiting = true;
            m_TimeSpentWaiting = m_WaitTime - 0.1f;
        }

        public HiraBotsTaskResult Execute(float deltaTime)
        {
            if (m_Waiting)
            {
                m_TimeSpentWaiting += deltaTime;

                if (m_TimeSpentWaiting >= m_WaitTime)
                {
                    m_Waiting = false;

                    m_NavMeshAgent.isStopped = false;
                    m_NavMeshAgent.speed = m_MoveSpeed;
                    m_NavMeshAgent.stoppingDistance = m_StopDistance;

                    bool destinationSet;
                    do
                    {
                        Vector3 destination;
                        var originalDestination = m_NavMeshAgent.destination;
                        do
                        {
                            destination = m_PatrolPointCollection.GetRandom();
                        } while (destination == originalDestination);

                        destinationSet = m_NavMeshAgent.SetDestination(destination);
                    } while (!destinationSet);
                }
            }
            else
            {
                if (m_NavMeshAgent.remainingDistance <= m_StopDistance)
                {
                    m_NavMeshAgent.isStopped = true;
                    m_Waiting = true;
                    m_TimeSpentWaiting = 0f;
                }
            }

            return HiraBotsTaskResult.InProgress;
        }

        public void Abort()
        {
            m_NavMeshAgent.isStopped = true;
            m_NavMeshAgent = null;

            m_PatrolPointCollection = null;
        }

        public void End(bool success)
        {
            // probably won't happen since we don't plan on ending this task ever
            // it will only ever be aborted when a blackboard update triggers re-plan
            m_NavMeshAgent = null;

            m_PatrolPointCollection = null;
        }
    }

    public class PatrolTaskProvider : HiraBotsTaskProvider
    {
        [SerializeField] private PatrolPointCollection m_PatrolPointCollection;
        [SerializeField] private float m_MoveSpeed;
        [SerializeField] private float m_StopDistance;
        [SerializeField] private float m_WaitTime;

        protected override IHiraBotsTask GetTask(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            if (archetype is not IHiraBotArchetype<NavMeshAgent> archetypeWithNavMeshAgent)
            {
                Debug.LogError($"Cannot provide a patrol task for {archetype.gameObject.name} because " +
                               $"it does not have a NavMeshComponent.");
                return null;
            }

            return new PatrolTask
            {
                m_NavMeshAgent = archetypeWithNavMeshAgent.component,
                m_PatrolPointCollection = m_PatrolPointCollection,
                m_MoveSpeed = m_MoveSpeed,
                m_StopDistance = m_StopDistance,
                m_WaitTime = m_WaitTime
            };
        }

        protected override void Validate(Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (m_PatrolPointCollection == null)
            {
                reportError("No patrol point collection present.");
            }
        }
    }
}