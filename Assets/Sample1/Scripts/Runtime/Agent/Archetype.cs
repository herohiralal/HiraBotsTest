using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class Archetype : MonoBehaviour, IHiraBotArchetype<NavMeshAgent>, IHiraBotArchetype<Animator>
    {
        [SerializeField] private Animator m_Animator = null;
        [SerializeField] private NavMeshAgent m_NavMeshAgent = null;

        Animator IHiraBotArchetype<Animator>.component => m_Animator;
        NavMeshAgent IHiraBotArchetype<NavMeshAgent>.component => m_NavMeshAgent;
    }
}