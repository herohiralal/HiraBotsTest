using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AIEngineTest
{
    public enum MeleeAttackResponseType
    {
        Normal,
        Ignore,
        OnlyStats,
    }

    public struct MeleeAttackStimulus
    {
        public CharacterAttributes m_InstigatorAttributes;
    }

    public class MeleeAttackResponseService : IHiraBotsService, IMessageListener<MeleeAttackStimulus>
    {
        public static MeleeAttackResponseService Get(CharacterAttributes selfAttributes, BlackboardComponent blackboard, MeleeAttackResponseType responseType)
        {
            var output = s_Executables.Count > 0 ? s_Executables.Pop() : new MeleeAttackResponseService();
            output.m_SelfAttributes = selfAttributes;
            output.m_Blackboard = blackboard;
            output.m_ResponseType = responseType;
            return output;
        }

        private MeleeAttackResponseService()
        {
        }

        private CharacterAttributes m_SelfAttributes;
        private BlackboardComponent m_Blackboard;
        private MeleeAttackResponseType m_ResponseType;

        private static readonly Stack<MeleeAttackResponseService> s_Executables = new Stack<MeleeAttackResponseService>();
        public void Start()
        {
        }

        public void Tick(float deltaTime)
        {
        }

        public void Stop()
        {
        }

        public void OnMessageReceived(MeleeAttackStimulus message)
        {
            if (m_ResponseType != MeleeAttackResponseType.Ignore)
            {
                var selfAttributes = m_SelfAttributes;
                var otherAttributes = message.m_InstigatorAttributes;

                var attackRoll = Random.Range(1, 21);
                var attackModifier = attackRoll + otherAttributes.attackModifier;
                var armourClass = selfAttributes.armourClass;

                if (attackModifier >= armourClass)
                {
                    Debug.Log($"{otherAttributes.gameObject.name} ({attackRoll} (1d20) + {attackModifier}) beat {selfAttributes.gameObject.name}'s armour class ({armourClass}).");

                    var (damageMin, damageMax, damageType) = otherAttributes.equippedWeaponDamageRange;
                    var damageVal = Random.Range(damageMin, damageMax + 1);

                    var (originalHitPoints, maxHitPoints) = selfAttributes.hitPoints;
                    var newHitPoints = Mathf.Clamp(originalHitPoints - damageVal, 0, maxHitPoints);

                    Debug.Log($"{selfAttributes.gameObject.name} (({originalHitPoints} -> {newHitPoints})/{maxHitPoints}) received {damageVal} {damageType} damage from {otherAttributes.gameObject.name}.");

                    selfAttributes.hitPoints = (newHitPoints, maxHitPoints);
                    // todo: particle fx here

                    switch (m_ResponseType)
                    {
                        case MeleeAttackResponseType.OnlyStats when newHitPoints > 0:
                            break;
                        case MeleeAttackResponseType.Normal:
                        case MeleeAttackResponseType.OnlyStats when newHitPoints == 0:
                            m_Blackboard.SetEnumValue("ReactionRequired", newHitPoints > 0 ? ReactionType.Hit : ReactionType.Die);
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    // todo: particle fx here

                    switch (m_ResponseType)
                    {
                        case MeleeAttackResponseType.Normal:
                            // m_Blackboard.SetEnumValue("ReactionRequired", ReactionType.AvoidHit);
                            break;
                        case MeleeAttackResponseType.OnlyStats:
                            break;
                        default:
                            throw new System.ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }

    public class MeleeAttackResponseServiceProvider : HiraBotsServiceProvider
    {
        public MeleeAttackResponseServiceProvider()
        {
            tickInterval = 10000f;
        }

        [SerializeField] private MeleeAttackResponseType m_ResponseType = MeleeAttackResponseType.Normal;

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return archetype is IHiraBotArchetype<CharacterAttributes> attributes
                ? MeleeAttackResponseService.Get(attributes.component, blackboard, m_ResponseType)
                : null;
        }

        protected override void UpdateDescription(out string staticDescription)
        {
            staticDescription = $"Response type: {m_ResponseType}.";
        }
    }
}