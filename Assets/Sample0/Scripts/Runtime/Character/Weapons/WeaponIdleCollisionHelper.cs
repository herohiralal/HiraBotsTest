using UnityEngine;

namespace AIEngineTest
{
    public class WeaponIdleCollisionHelper : MonoBehaviour
    {
        [SerializeField] private Collider m_Collider;
        [SerializeField] private Rigidbody m_Rigidbody;

        private void OnEnable()
        {
            m_Collider.enabled = true;

            m_Rigidbody.useGravity = true;
            m_Rigidbody.isKinematic = false;
        }

        private void OnDisable()
        {
            m_Collider.enabled = false;

            m_Rigidbody.useGravity = false;
            m_Rigidbody.isKinematic = true;
        }
    }
}