using UnityEngine;

namespace AIEngineTest
{
    [CreateAssetMenu(fileName = "NewPatrolPointCollection", menuName = "Samples/1/Patrol Point Collection", order = 0)]
    internal class PatrolPointCollection : ScriptableObject
    {
        [System.NonSerialized] private Transform[] m_PatrolPoints = new Transform[4];
        [System.NonSerialized] private int m_Count = 0;

        public void Add(PatrolPoint point)
        {
            var t = point.transform;

            if (m_Count == m_PatrolPoints.Length)
            {
                System.Array.Resize(ref m_PatrolPoints, m_Count * 2);
            }

            m_PatrolPoints[m_Count] = t;

            m_Count++;
        }

        public void Remove(PatrolPoint point)
        {
            var t = point.transform;

            var removeAt = -1;
            for (var i = 0; i < m_Count; i++)
            {
                if (m_PatrolPoints[i] == t)
                {
                    removeAt = i;
                    break;
                }
            }

            if (removeAt != -1)
            {
                m_Count--;

                m_PatrolPoints[removeAt] = m_PatrolPoints[m_Count];
                m_PatrolPoints[m_Count] = null;
            }
        }

        public Vector3 GetRandom(Vector3 defaultValue = default)
        {
            return m_Count > 0 ? m_PatrolPoints[Random.Range(0, m_Count)].position : defaultValue;
        }
    }
}