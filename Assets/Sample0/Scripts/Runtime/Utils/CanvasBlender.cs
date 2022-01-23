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

        public IEnumerator Blend(bool target)
        {
            gameObject.SetActive(true);

            var start = target ? 0f : 1f;
            var end = 1f - start;

            for (var timeElapsed = 0f; timeElapsed < m_BlendTime; timeElapsed += Time.unscaledDeltaTime)
            {
                m_CanvasGroup.alpha = Mathf.Lerp(start, end, timeElapsed / m_BlendTime);
                yield return null;
            }

            m_CanvasGroup.alpha = end;

            if (!target)
            {
                gameObject.SetActive(false);
            }
        }
    }
}