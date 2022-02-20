using UnityEngine;

namespace AIEngineTest
{
    public class MarkupPoint : MonoBehaviour
    {
        [SerializeField] private MarkupPointCollection m_Collection = null;

        public MarkupPointCollection collection
        {
            get => m_Collection;
            set
            {
                var activeAndEnabled = isActiveAndEnabled;

                if (activeAndEnabled)
                {
                    if (!ReferenceEquals(m_Collection, null))
                    {
                        m_Collection.Remove(this);
                    }
                }

                m_Collection = value;

                if (activeAndEnabled)
                {
                    if (!ReferenceEquals(m_Collection, null))
                    {
                        m_Collection.Add(this);
                    }
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