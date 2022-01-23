using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace AIEngineTest
{
    public class SoldierAnimatorEventsListener : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_OnFootL;
        [SerializeField] private UnityEvent m_OnFootR;

        [Preserve]
        public void FootL()
        {
            m_OnFootL.Invoke();
        }

        [Preserve]
        public void FootR()
        {
            m_OnFootR.Invoke();
        }
    }
}