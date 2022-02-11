using System.Collections;
using UnityEngine;

namespace AIEngineTest
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField] private CanvasBlender m_CanvasBlenderForPauseButton;
        [SerializeField] private CanvasBlender m_CanvasBlenderForPauseMenu;

        public IEnumerator Blend(bool target)
        {
            return InterpolationHelper.Parallel(
                m_CanvasBlenderForPauseButton.Blend(!target),
                m_CanvasBlenderForPauseMenu.Blend(target));
        }
    }
}