using System.Collections.Generic;
using UnityEngine;

namespace AIEngineTest
{
    public class ParticleFXPoolManager : MonoBehaviour
    {
        private static readonly Dictionary<string, ParticleFXPool> s_FXPoolManagers = new Dictionary<string, ParticleFXPool>();

        [SerializeField] public ParticleFXPool m_Pool;

        [System.NonSerialized] private bool m_Valid;

        private void OnEnable()
        {
            m_Pool.Clear();
            if (s_FXPoolManagers.TryAdd(m_Pool.name, m_Pool))
            {
                m_Valid = true;
            }
            else
            {
                Debug.LogError($"Cannot add duplicate pool {m_Pool.name}.", m_Pool);
                m_Valid = false;
            }
        }

        private void OnDisable()
        {
            if (m_Valid)
            {
                s_FXPoolManagers.Remove(m_Pool.name);
            }

            m_Valid = false;
            m_Pool.Clear();
        }

        public static bool TryGet(string poolName, out ParticleFXPool pool)
        {
            return s_FXPoolManagers.TryGetValue(poolName, out pool);
        }
    }
}