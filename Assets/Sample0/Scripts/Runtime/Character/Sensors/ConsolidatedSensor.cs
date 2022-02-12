using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace AIEngineTest
{
    public class ConsolidatedSensor : MonoBehaviour
    {
        [SerializeField] private HiraBotSensor.NewObjectPerceivedEvent m_OnNewObjectPerceived;
        [SerializeField] private HiraBotSensor.ObjectStoppedPerceivingEvent m_OnObjectStoppedPerceiving;

        private readonly HashSet<Object> m_Objects = new HashSet<Object>();

        public HiraBotSensor.NewObjectPerceivedEvent newObjectPerceived => m_OnNewObjectPerceived;
        public HiraBotSensor.ObjectStoppedPerceivingEvent objectStoppedPerceiving => m_OnObjectStoppedPerceiving;
        public ReadOnlyHashSetAccessor<Object> perceivedObjects => m_Objects.ReadOnly();

        private void OnDestroy()
        {
            m_Objects.Clear();
        }

        public void Found(Object o)
        {
            if (m_Objects.Add(o))
            {
                try
                {
                    m_OnNewObjectPerceived.Invoke(o);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public void Lost(Object o)
        {
            if (m_Objects.Remove(o))
            {
                try
                {
                    m_OnObjectStoppedPerceiving.Invoke(o);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}