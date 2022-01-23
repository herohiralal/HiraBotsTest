using UnityEngine;
using UnityEngine.AI;

namespace AIEngineTest
{
    public class Archetype : MonoBehaviour, IHiraBotArchetype<NavMeshAgent>
    {
        [SerializeField] private NavMeshAgent m_NavMeshAgent = null;

        NavMeshAgent IHiraBotArchetype<NavMeshAgent>.component => m_NavMeshAgent;
    }
}