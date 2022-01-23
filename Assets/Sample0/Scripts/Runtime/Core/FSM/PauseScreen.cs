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
            var a = m_CanvasBlenderForPauseButton.Blend(!target);
            var b = m_CanvasBlenderForPauseMenu.Blend(target);

            var aMoveNext = a.MoveNext();
            var bMoveNext = b.MoveNext();

            while (aMoveNext || bMoveNext)
            {
                aMoveNext = a.MoveNext();
                bMoveNext = b.MoveNext();

                yield return null;
            }
        }
    }
}