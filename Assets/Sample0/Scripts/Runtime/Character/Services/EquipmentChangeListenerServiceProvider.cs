using System.Collections.Generic;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class EquipmentChangeListenerService : IHiraBotsService
    {
        
        public static EquipmentChangeListenerService Get(BlackboardComponent blackboard, AnimatorHelper animator)
        {
            var output = s_Executables.Count > 0 ? s_Executables.Pop() : new EquipmentChangeListenerService();
            output.m_Blackboard = blackboard;
            output.m_Animator = animator;
            return output;
        }

        private EquipmentChangeListenerService()
        {
        }

        private BlackboardComponent m_Blackboard;
        private AnimatorHelper m_Animator;

        private static readonly Stack<EquipmentChangeListenerService> s_Executables = new Stack<EquipmentChangeListenerService>();

        public void Start()
        {
            m_Animator.equip.AddListener(OnEquipmentChange);
        }

        private void OnEquipmentChange(EquipmentType newEquipment)
        {
            m_Blackboard.SetEnumValue("CurrentEquipment", newEquipment);
            m_Blackboard.SetBooleanValue("HasWeaponEquipped", newEquipment != EquipmentType.None);
        }

        public void Stop()
        {
            m_Animator.equip.RemoveListener(OnEquipmentChange);
            m_Blackboard = default;
            m_Animator = null;
        }

        public void Tick(float deltaTime)
        {
            // no op
        }
    }

    public class EquipmentChangeListenerServiceProvider : HiraBotsServiceProvider
    {
        public EquipmentChangeListenerServiceProvider()
        {
            tickInterval = 100f;
        }

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return archetype is IHiraBotArchetype<AnimatorHelper> animated
                ? EquipmentChangeListenerService.Get(blackboard, animated.component)
                : null;
        }
    }
}