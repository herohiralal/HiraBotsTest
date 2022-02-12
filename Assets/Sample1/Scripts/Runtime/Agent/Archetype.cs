using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class Archetype : MonoBehaviour,
        IHiraBotArchetype<NavMeshAgent>,
        IHiraBotArchetype<Animator>,
        IHiraBotArchetype<ConsolidatedSensor>,
        IHiraBotArchetype<HiraLGOAPRealtimeBot>
    {
        [SerializeField] private Animator m_Animator = null;
        [SerializeField] private NavMeshAgent m_NavMeshAgent = null;
        [SerializeField] private ConsolidatedSensor m_Sensor = null;
        [SerializeField] private HiraLGOAPRealtimeBot m_Brain = null;

        Animator IHiraBotArchetype<Animator>.component => m_Animator;
        NavMeshAgent IHiraBotArchetype<NavMeshAgent>.component => m_NavMeshAgent;
        ConsolidatedSensor IHiraBotArchetype<ConsolidatedSensor>.component => m_Sensor;
        HiraLGOAPRealtimeBot IHiraBotArchetype<HiraLGOAPRealtimeBot>.component => m_Brain;
    }
}