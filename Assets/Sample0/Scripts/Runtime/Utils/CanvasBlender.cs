using System.Collections;
using UnityEngine;

namespace AIEngineTest
{
    public class CanvasBlender : MonoBehaviour
    {
        [SerializeField] private float m_BlendTime;
        [SerializeField] private CanvasGroup m_CanvasGroup;
        [SerializeField] private bool m_StartDisabled;

        private void Awake()
        {
            gameObject.SetActive(!m_StartDisabled);
        }

        public IEnumerator Blend(bool enable)
        {
            if (enable)
            {
                gameObject.SetActive(true);
            }

            yield return m_CanvasGroup.TweenAlpha<LinearInterpolator, UnscaledTime>(enable ? 0f : 1f, enable ? 1f : 0f, m_BlendTime);

            if (!enable)
            {
                gameObject.SetActive(false);
            }
        }
    }
}