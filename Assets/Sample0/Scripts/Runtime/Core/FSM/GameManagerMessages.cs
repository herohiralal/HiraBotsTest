using UnityEngine;

namespace AIEngineTest
{
    public class GameManagerMessages : MonoBehaviour
    {
        [System.Serializable]
        private enum Type
        {
            None,
            ClickPlay,
            QuitFromMainMenu,
            QuitFromDebugScene,
            Pause,
            Resume,
            QuitToMainMenuFromDebugScene,
        }

        [SerializeField] private Type m_MessageType = Type.None;

        public void Trigger()
        {
            switch (m_MessageType)
            {
                case Type.None:
                    Debug.Log("Empty message.", this);
                    break;
                case Type.ClickPlay:
                    GameManager.OnClickPlay();
                    break;
                case Type.QuitFromMainMenu:
                    GameManager.OnQuitFromMainMenu();
                    break;
                case Type.QuitFromDebugScene:
                    GameManager.OnQuitFromDebugScene();
                    break;
                case Type.Pause:
                    GameManager.OnPause();
                    break;
                case Type.Resume:
                    GameManager.OnResume();
                    break;
                case Type.QuitToMainMenuFromDebugScene:
                    GameManager.OnQuitToMainMenuFromDebugScene();
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
        }
    }
}