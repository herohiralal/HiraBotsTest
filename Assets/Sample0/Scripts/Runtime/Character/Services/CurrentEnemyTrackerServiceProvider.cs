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

        private static readonly Stack<CurrentEnemyTrackerService> s_Executables = new Stack<CurrentEnemyTrackerService>();

        public void Start()
        {
            // no op
        }

        public void Tick(float deltaTime)
        {
            Run(m_Self, m_Blackboard);
        }

        public static void Run(IHiraBotArchetype self, BlackboardComponent blackboard)
        {
            TargetRelativeDistance trd;
            TargetRelativePosition trp;

            if (blackboard.GetObjectValue("TargetEnemy") is not IHiraBotArchetype target)
            {
                trp = TargetRelativePosition.Irrelevant;
                trd = TargetRelativeDistance.Irrelevant;
            }
            else
            {
                var selfTransform = self.gameObject.transform;
                var otherTransform = target.gameObject.transform;

                var selfPos = selfTransform.position;
                var otherPos = otherTransform.position;

                {
                    var dist = Vector3.Distance(selfPos, otherPos);
                    trd = dist switch
                    {
                        <= 2.0f => TargetRelativeDistance.Melee,
                        <= 7.5f => TargetRelativeDistance.Short,
                        _ => TargetRelativeDistance.Long
                    };
                }

                {
                    var dot = Vector3.Dot(otherTransform.forward, (selfPos - otherPos).normalized);
                    trp = dot switch
                    {
                        >= 0.5f => TargetRelativePosition.InFront,
                        >= -0.5f => TargetRelativePosition.Flanking,
                        _ => TargetRelativePosition.Sneaking
                    };
                }
            }

            blackboard.SetEnumValue("DistanceToEnemy", trd);
            blackboard.SetEnumValue("EnemyRelativePosition", trp);
        }

        public void Stop()
        {
            Run(m_Self, m_Blackboard);

            m_Blackboard = default;
            s_Executables.Push(this);
        }
    }

    public class CurrentEnemyTrackerServiceProvider : HiraBotsServiceProvider
    {
        public CurrentEnemyTrackerServiceProvider()
        {
            tickInterval = 0.1f;
        }

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return CurrentEnemyTrackerService.Get(archetype, blackboard);
        }
    }
}