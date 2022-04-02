using UnityEngine;

namespace AIEngineTest
{
    public class WeaponArchetype : MonoBehaviour
    {
        [SerializeField] public MeshFilter m_MeshFilter;
        [SerializeField] public MeshRenderer m_MeshRenderer;
        [SerializeField] public BoxCollider m_Collider;
        [SerializeField] public Rigidbody m_Rigidbody;
        [SerializeField] public WeaponIdleCollisionHelper m_IdleCollisionHelper;
        [SerializeField] public WeaponAttackCollisionHelper m_AttackCollisionHelper;
    }
}