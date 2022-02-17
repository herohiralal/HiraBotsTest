using UnityEngine;

namespace AIEngineTest
{
    public abstract class ObjectCollection<T> : ScriptableObject where T : Object
    {
        [SerializeField] private T[] m_Objects = new T[0];

        public T[] objects
        {
            get => m_Objects;
            set => m_Objects = value;
        }

        public T GetRandom()
        {
            return m_Objects[Random.Range(0, m_Objects.Length)];
        }
    }
}