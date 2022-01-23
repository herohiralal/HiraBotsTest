using UnityEngine;

namespace AIEngineTest
{
    public class PatrolPoint : MonoBehaviour
    {
        [SerializeField] private PatrolPointCollection m_Collection = null;

        public PatrolPointCollection collection
        {
            get => m_Collection;
            set
            {
                if (!ReferenceEquals(m_Collection, null))
                {
                    m_Collection.Remove(this);
                }

                m_Collection = value;

                if (!ReferenceEquals(m_Collection, null))
                {
                    m_Collection.Add(this);
                }
            }
        }

        private void OnEnable()
        {
            if (!ReferenceEquals(m_Collection, null))
            {
                m_Collection.Add(this);
            }
        }

        private void OnDisable()
        {
            if (!ReferenceEquals(m_Collection, null))
            {
                m_Collection.Remove(this);
            }
        }
    }
}