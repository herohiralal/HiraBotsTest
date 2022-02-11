using System.Collections;
using UnityEngine;

namespace AIEngineTest
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField] private CanvasBlender m_CanvasBlenderForPauseButton;
        [SerializeField] private CanvasBlender m_CanvasBlenderForPauseMenu;

        public IEnumerator Blend(bool target, float? blendTime = null)
        {
            return this.Parallel(
                m_CanvasBlenderForPauseButton.Blend(!target, blendTime),
                m_CanvasBlenderForPauseMenu.Blend(target, blendTime));
        }
    }
}