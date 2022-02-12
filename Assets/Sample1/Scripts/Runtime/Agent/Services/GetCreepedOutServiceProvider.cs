using System;
using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class GetCreepedOutService : IHiraBotsService, IMessageListener<FanGreetingMessage>
    {
        public BlackboardComponent m_Blackboard;
        public string m_CreepedOutByKey;
        public int m_Count;

        public void Start()
        {
        }

        public void Tick(float deltaTime)
        {
        }

        public void Stop()
        {
            m_Blackboard = default;
        }

        public void OnMessageReceived(FanGreetingMessage message)
        {
            m_Count--;
            if (m_Count == 0)
            {
                m_Blackboard.SetObjectValue(m_CreepedOutByKey, message.m_Fan);
            }
        }
    }

    public class GetCreepedOutServiceProvider : HiraBotsServiceProvider
    {
        public GetCreepedOutServiceProvider()
        {
            tickInterval = 1000f;
        }

        [SerializeField] private BlackboardTemplate.KeySelector m_CreepedOutBy;
        [SerializeField] private int m_TimesGreetedBeforeCreepedOut = 3;

        #region Validation Boilerplate

        protected override void OnValidateCallback()
        {
            m_CreepedOutBy.keyTypesFilter = BlackboardKeyType.Object;
        }

        protected override void Validate(Action<string> reportError, in BlackboardTemplate.KeySet keySet)
        {
            if (!m_CreepedOutBy.Validate(keySet, BlackboardKeyType.Object))
            {
                reportError("no creeped out by key");
            }
        }

        protected override void OnTargetBlackboardTemplateChanged(BlackboardTemplate template, in BlackboardTemplate.KeySet keySet)
        {
            m_CreepedOutBy.OnTargetBlackboardTemplateChanged(template, in keySet);
        }

        #endregion

        protected override IHiraBotsService GetService(BlackboardComponent blackboard, IHiraBotArchetype archetype)
        {
            return new GetCreepedOutService
            {
                m_Blackboard = blackboard,
                m_CreepedOutByKey = m_CreepedOutBy.selectedKey.name,
                m_Count = m_TimesGreetedBeforeCreepedOut
            };
        }
    }
}