using System.Collections.Generic;
using UnityEngine;

namespace AIEngineTest
{
    public class WeaponCollisionTracker : MonoBehaviour
    {
        public HashSet<BaseArchetype> collidingCharacters { get; } = new HashSet<BaseArchetype>();

        public void Clear()
        {
            collidingCharacters.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            var otherArchetype = other.transform.root.GetComponent<BaseArchetype>();
            if (otherArchetype != null)
            {
                collidingCharacters.Add(otherArchetype);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var otherArchetype = other.transform.root.GetComponent<BaseArchetype>();
            if (otherArchetype != null)
            {
                collidingCharacters.Remove(otherArchetype);
            }
        }
    }
}