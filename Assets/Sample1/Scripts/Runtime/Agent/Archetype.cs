using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class Archetype : MonoBehaviour,
        IHiraBotArchetype<NavMeshAgent>,
        IHiraBotArchetype<Animator>,
        IHiraBotArchetype<ConsolidatedSensor>,
        IHiraBotArchetype<HiraLGOAPRealtimeBot>,
        IHiraBotArchetype<SightStimulus>,
        IHiraBotArchetype<SoundStimulus>
    {
        [SerializeField] private Animator m_Animator = null;
        [SerializeField] private NavMeshAgent m_NavMeshAgent = null;
        [SerializeField] private ConsolidatedSensor m_Sensor = null;
        [SerializeField] private HiraLGOAPRealtimeBot m_Brain = null;
        [SerializeField] private SightStimulus m_SightStimulus = null;
        [SerializeField] private SoundStimulus m_SoundStimulus = null;

        Animator IHiraBotArchetype<Animator>.component => m_Animator;
        NavMeshAgent IHiraBotArchetype<NavMeshAgent>.component => m_NavMeshAgent;
        ConsolidatedSensor IHiraBotArchetype<ConsolidatedSensor>.component => m_Sensor;
        HiraLGOAPRealtimeBot IHiraBotArchetype<HiraLGOAPRealtimeBot>.component => m_Brain;
        SightStimulus IHiraBotArchetype<SightStimulus>.component => m_SightStimulus;
        SoundStimulus IHiraBotArchetype<SoundStimulus>.component => m_SoundStimulus;
    }
}