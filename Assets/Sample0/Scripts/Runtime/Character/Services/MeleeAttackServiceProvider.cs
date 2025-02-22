﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class MeleeAttackService : IHiraBotsService
    {
        public static MeleeAttackService Get(CharacterAttributes attributes, AnimatorHelper animator, CharacterMeshWeaponSocketProvider socketProvider, BlackboardComponent blackboard)
        {
            var output = s_Executables.Count > 0 ? s_Executables.Pop() : new MeleeAttackService();
            output.m_Attributes = attributes;
            output.m_Animator = animator;
            output.m_SocketProvider = socketProvider;
            output.m_Blackboard = blackboard;
            return output;
        }

        private MeleeAttackService()
        {
        }

        private CharacterAttributes m_Attributes;
        private AnimatorHelper m_Animator;
        private CharacterMeshWeaponSocketProvider m_SocketProvider;
        private BlackboardComponent m_Blackboard;

        private static readonly Stack<MeleeAttackService> s_Executables = new Stack<MeleeAttackService>();

        public void Start()
        {
            m_Animator.hitLStart.AddListener(HitLStart);
            m_Animator.hitLStop.AddListener(HitLStop);
            m_Animator.hitRStart.AddListener(HitRStart);
            m_Animator.hitRStop.AddListener(HitRStop);
        }

        private void HitLStart()
        {
            if (m_SocketProvider.GetEquippedWeaponL(out var weapon))
            {
                weapon.m_AttackCollisionHelper.enabled = true;
                weapon.m_AttackCollisionHelper.hit.AddListener(OnAttack);
            }
        }

        private void HitLStop()
        {
            if (m_SocketProvider.GetEquippedWeaponL(out var weapon))
            {
                weapon.m_AttackCollisionHelper.hit.RemoveListener(OnAttack);
                weapon.m_AttackCollisionHelper.enabled = false;
            }
        }

        private void HitRStart()
        {
            if (m_SocketProvider.GetEquippedWeaponR(out var weapon))
            {
                weapon.m_AttackCollisionHelper.enabled = true;
                weapon.m_AttackCollisionHelper.hit.AddListener(OnAttack);
            }
        }

        private void HitRStop()
        {
            if (m_SocketProvider.GetEquippedWeaponR(out var weapon))
            {
                weapon.m_AttackCollisionHelper.hit.RemoveListener(OnAttack);
                weapon.m_AttackCollisionHelper.enabled = false;
            }
        }

        public void Tick(float deltaTime)
        {
            // no op
        }

        public void Stop()
        {
            HitLStop();
            HitRStop();

            m_Animator.hitLStart.RemoveListener(HitLStart);
            m_Animator.hitLStop.RemoveListener(HitLStop);
            m_Animator.hitRStart.RemoveListener(HitRStart);
            m_Animator.hitRStop.RemoveListener(HitRStop);

            m_Attributes = null;
            m_Animator = null;
            m_SocketProvider = null;
            m_Blackboard = default;
        }

        private unsafe void OnAttack(BaseArchetype target, Vector3 contactPoint)
        {
            // don't damage self
            if (target.m_CharacterAttributes == m_Attributes)
            {
                return;
            }

            MeleeAttackResult result = default;
            target.m_Brain.Message<MeleeAttackStimulus>(new MeleeAttackStimulus
            {
                m_InstigatorAttributes = m_Attributes,
                m_ContactPoint = contactPoint,
                m_Result = &result
            });

            if ((result.m_Flags & MeleeAttackResultFlags.VictimDead) != 0)
            {
                m_Blackboard.SetObjectValue("TargetEnemy", null);
            }
        }
    }

    public class MeleeAttackServiceProvider : HiraBotsServiceProvider
    {
        public MeleeAttackServiceProvider()
        {
            tickInterval = 10000f;
        }

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return archetype is
                IHiraBotArchetype<CharacterAttributes> attributes
                and IHiraBotArchetype<AnimatorHelper> animator
                and IHiraBotArchetype<CharacterMeshWeaponSocketProvider> socketProvider
                ? MeleeAttackService.Get(attributes.component, animator.component, socketProvider.component, blackboard)
                : null;
        }
    }
}