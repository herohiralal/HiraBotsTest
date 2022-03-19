using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class HiraBotsGlobalTimeUpdater : MonoBehaviour
    {
        [SerializeField] private BlackboardTemplate m_BlackboardTemplate;
        [SerializeField] private float m_UpdateTime = 0.1f;
        [System.NonSerialized] private float m_DeltaTime = 0f;

        private void OnEnable()
        {
            m_DeltaTime = 0f;
        }

        private void Update()
        {
            m_DeltaTime += Time.deltaTime;
            if (m_DeltaTime >= m_UpdateTime)
            {
                m_DeltaTime = 0f;

                m_BlackboardTemplate.SetInstanceSyncedFloatValue("GlobalTime", Time.time);
            }
        }
    }
}