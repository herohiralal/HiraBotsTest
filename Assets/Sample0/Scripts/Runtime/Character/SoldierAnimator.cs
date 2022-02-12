using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Scripting;

namespace AIEngineTest
{
    public class SoldierAnimator : MonoBehaviour
    {
        [SerializeField] private UnityEvent m_OnFootL;
        [SerializeField] private UnityEvent m_OnFootR;
        [SerializeField] private UnityEvent m_OnBow;

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

        [Preserve]
        public void Bow()
        {
            m_OnBow.Invoke();
        }
    }
}