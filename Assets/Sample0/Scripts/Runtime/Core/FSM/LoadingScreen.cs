using UnityEngine;

namespace AIEngineTest
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private CanvasBlender m_CanvasBlender;
        public CanvasBlender canvasBlender => m_CanvasBlender;

        [SerializeField] private RectTransform m_LoadingValue;

        public float loadingValue
        {
            get
            {
                if (m_LoadingValue == null)
                {
                    return 0f;
                }

                return m_LoadingValue.localScale.x;
            }
            set
            {
                if (m_LoadingValue == null)
                {
                    return;
                }

                var scale = m_LoadingValue.localScale;
                scale.x = Mathf.Clamp01(value);
                m_LoadingValue.localScale = scale;
            }
        }
    }
}