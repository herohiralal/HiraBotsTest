using UnityEngine;

namespace AIEngineTest
{
    public class OneShotParticleFX : MonoBehaviour
    {
        public float m_Timer = 1f;
        public ParticleSystem m_ParticleSystem = null;

        private void Reset()
        {
            m_Timer = 1f;
            m_ParticleSystem = GetComponent<ParticleSystem>();
        }
    }
}