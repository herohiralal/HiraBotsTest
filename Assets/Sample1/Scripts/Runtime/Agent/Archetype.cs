using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class Archetype : MonoBehaviour,
        IHiraBotArchetype<NavMeshAgent>,
        IHiraBotArchetype<Animator>,
        IHiraBotArchetype<SightSensor>,
        IHiraBotArchetype<SoundSensor>
    {
        [SerializeField] private Animator m_Animator = null;
        [SerializeField] private NavMeshAgent m_NavMeshAgent = null;
        [SerializeField] private SightSensor m_SightSensor = null;
        [SerializeField] private SoundSensor m_SoundSensor = null;

        Animator IHiraBotArchetype<Animator>.component => m_Animator;
        NavMeshAgent IHiraBotArchetype<NavMeshAgent>.component => m_NavMeshAgent;
        SightSensor IHiraBotArchetype<SightSensor>.component => m_SightSensor;
        SoundSensor IHiraBotArchetype<SoundSensor>.component => m_SoundSensor;
        
    }
}