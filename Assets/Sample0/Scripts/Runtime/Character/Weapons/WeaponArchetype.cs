using UnityEngine;

namespace AIEngineTest
{
    public class WeaponArchetype : MonoBehaviour
    {
        [SerializeField] public MeshFilter m_MeshFilter;
        [SerializeField] public MeshRenderer m_MeshRenderer;
        [SerializeField] public BoxCollider m_Collider;
    }
}