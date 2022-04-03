using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AIEngineTest
{
    public class WeaponAttackCollisionHelper : MonoBehaviour
    {
        [SerializeField] private Collider m_Collider;
        [SerializeField] private Rigidbody m_Rigidbody;

        [SerializeField] private UnityEvent<BaseArchetype, Vector3> m_OnHit;
        public UnityEvent<BaseArchetype, Vector3> hit => m_OnHit;

        private readonly HashSet<GameObject> m_AttackedGameObjects = new HashSet<GameObject>();

        private void OnEnable()
        {
            m_AttackedGameObjects.Clear();
            m_OnHit.RemoveAllListeners();

            m_Collider.enabled = true;
            m_Collider.isTrigger = false;

            m_Rigidbody.useGravity = false;
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        private void OnDisable()
        {
            m_Collider.enabled = false;

            m_Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;

            m_OnHit.RemoveAllListeners();
            m_AttackedGameObjects.Clear();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            var other = collision.collider.transform.root.gameObject;
            if (!m_AttackedGameObjects.Add(other))
            {
                return;
            }

            if (other.TryGetComponent<BaseArchetype>(out var otherArchetype))
            {
                m_OnHit.Invoke(otherArchetype, collision.GetContact(0).point);
            }
        }
    }
}