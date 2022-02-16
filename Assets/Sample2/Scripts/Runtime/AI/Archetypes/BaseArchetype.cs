using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public abstract class BaseArchetype : MonoBehaviour,
        IHiraBotArchetype<AnimatorHelper>,
        IHiraBotArchetype<NavMeshAgent>,
        IHiraBotArchetype<ConsolidatedSensor>,
        IHiraBotArchetype<HiraLGOAPRealtimeBot>,
        IHiraBotArchetype<SightStimulus>,
        IHiraBotArchetype<SoundStimulus>
    {
        [SerializeField] public AnimatorHelper m_AnimatorHelper = null;
        [SerializeField] public NavMeshAgent m_NavMeshAgent = null;
        [SerializeField] public ConsolidatedSensor m_Sensor = null;
        [SerializeField] public HiraLGOAPRealtimeBot m_Brain = null;
        [SerializeField] public SightStimulus m_SightStimulus = null;
        [SerializeField] public SoundStimulus m_SoundStimulus = null;

        AnimatorHelper IHiraBotArchetype<AnimatorHelper>.component => m_AnimatorHelper;
        NavMeshAgent IHiraBotArchetype<NavMeshAgent>.component => m_NavMeshAgent;
        ConsolidatedSensor IHiraBotArchetype<ConsolidatedSensor>.component => m_Sensor;
        HiraLGOAPRealtimeBot IHiraBotArchetype<HiraLGOAPRealtimeBot>.component => m_Brain;
        SightStimulus IHiraBotArchetype<SightStimulus>.component => m_SightStimulus;
        SoundStimulus IHiraBotArchetype<SoundStimulus>.component => m_SoundStimulus;
    }
}