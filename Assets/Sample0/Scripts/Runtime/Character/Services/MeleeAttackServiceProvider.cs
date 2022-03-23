using System.Collections.Generic;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class MeleeAttackService : IHiraBotsService
    {
        public static MeleeAttackService Get(CharacterAttributes attributes, AnimatorHelper animator)
        {
            var output = s_Executables.Count > 0 ? s_Executables.Pop() : new MeleeAttackService();
            output.m_Attributes = attributes;
            output.m_Animator = animator;
            return output;
        }

        private MeleeAttackService()
        {
        }

        private CharacterAttributes m_Attributes;
        private AnimatorHelper m_Animator;

        private static readonly Stack<MeleeAttackService> s_Executables = new Stack<MeleeAttackService>();

        public void Start()
        {
            m_Animator.hit.AddListener(OnAttack);
        }

        public void Tick(float deltaTime)
        {
            // no op
        }

        public void Stop()
        {
            m_Animator.hit.RemoveListener(OnAttack);

            m_Attributes = null;
            m_Animator = null;
        }

        private void OnAttack()
        {
            // todo: figure out the target to attack - collider usage or raycast or just direct damage
        }
    }

    public class MeleeAttackServiceProvider : HiraBotsServiceProvider
    {
        public MeleeAttackServiceProvider()
        {
            tickInterval = 100f;
        }

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return archetype is IHiraBotArchetype<CharacterAttributes> attributes and IHiraBotArchetype<AnimatorHelper> animator
                ? MeleeAttackService.Get(attributes.component, animator.component)
                : null;
        }
    }
}