using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class CurrentEnemyTrackerService : IHiraBotsService
    {
        public static CurrentEnemyTrackerService Get(IHiraBotArchetype self, BlackboardComponent blackboard)
        {
            var output = s_Executables.Count > 0 ? s_Executables.Pop() : new CurrentEnemyTrackerService();
            output.m_Self = self;
            output.m_Blackboard = blackboard;
            return output;
        }

        private CurrentEnemyTrackerService()
        {
        }

        private IHiraBotArchetype m_Self;
        private BlackboardComponent m_Blackboard;

        private float m_MeleeDistance;
        private float m_ShortDistance;

        private static readonly Stack<CurrentEnemyTrackerService> s_Executables = new Stack<CurrentEnemyTrackerService>();

        public void Start()
        {
            // no op
        }

        public void Tick(float deltaTime)
        {
            TargetRelativeDistance trd;
            TargetRelativePosition trp;

            if (m_Blackboard.GetObjectValue("TargetEnemy") is not IHiraBotArchetype target)
            {
                trp = TargetRelativePosition.Irrelevant;
                trd = TargetRelativeDistance.Irrelevant;
            }
            else
            {
                var selfTransform = m_Self.gameObject.transform;
                var otherTransform = target.gameObject.transform;

                var selfPos = selfTransform.position;
                var otherPos = otherTransform.position;

                {
                    var dist = Vector3.Distance(selfPos, otherPos);
                    if (dist <= m_MeleeDistance)
                    {
                        trd = TargetRelativeDistance.Melee;
                    }
                    else if (dist <= m_ShortDistance)
                    {
                        trd = TargetRelativeDistance.Short;
                    }
                    else
                    {
                        trd = TargetRelativeDistance.Long;
                    }
                }

                {
                    var dot = Vector3.Dot(otherTransform.forward, (selfPos - otherPos).normalized);
                    if (dot >= 0.5f)
                    {
                        trp = TargetRelativePosition.InFront;
                    }
                    else if (dot >= -0.5f)
                    {
                        trp = TargetRelativePosition.Flanking;
                    }
                    else
                    {
                        trp = TargetRelativePosition.Sneaking;
                    }
                }
            }

            m_Blackboard.SetEnumValue("DistanceToEnemy", trd);
            m_Blackboard.SetEnumValue("EnemyRelativePosition", trp);
        }

        public void Stop()
        {
            m_Blackboard = default;
            s_Executables.Push(this);
        }
    }

    public class CurrentEnemyTrackerServiceProvider : HiraBotsServiceProvider
    {
        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return CurrentEnemyTrackerService.Get(archetype, blackboard);
        }
    }
}