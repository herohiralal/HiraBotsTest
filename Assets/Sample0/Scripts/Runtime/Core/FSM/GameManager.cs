using System.Collections;
using AIEngineTest;
using UnityEngine;
using UnityEngine.SceneManagement;

// if this class is put in a namespace,
// we won't get the cool icon for the script
public class GameManager : MonoBehaviour
{
    private static GameManager s_Instance;
    private void Awake() => s_Instance = this;

    [SerializeField] private Camera m_MainCamera = null;
    [SerializeField] private LoadingScreen m_LoadingScreenPrefab = null;
    [SerializeField] private PauseScreen m_PauseScreenPrefab;

    [System.NonSerialized] private LoadingScreen m_CurrentLoadingScreen;
    [System.NonSerialized] private PauseScreen m_CurrentPauseScreen;

    [System.NonSerialized] private bool m_Locked = false;

    public new static Camera camera => s_Instance.m_MainCamera;
    public static LoadingScreen currentLoadingScreen => s_Instance.m_CurrentLoadingScreen;
    public static PauseScreen currentPauseScreen => s_Instance.m_CurrentPauseScreen;

    public static ref bool locked => ref s_Instance.m_Locked;

    #region Scene Management Utils

    private static IEnumerator LoadScene(int buildIndex, float loaderMin, float loaderMax)
    {
        var asyncOp = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
        while (!asyncOp.isDone)
        {
            currentLoadingScreen.loadingValue = Mathf.Lerp(loaderMin, loaderMax, asyncOp.progress);
            yield return null;
        }
    }

    private static IEnumerator UnloadScene(int buildIndex, float loaderMin, float loaderMax)
    {
        var asyncOp = SceneManager.UnloadSceneAsync(buildIndex, UnloadSceneOptions.None);
        while (!asyncOp.isDone)
        {
            currentLoadingScreen.loadingValue = Mathf.Lerp(loaderMin, loaderMax, asyncOp.progress);
            yield return null;
        }
    }

    private static IEnumerator SetActiveScene(int buildIndex)
    {
        var scene = SceneManager.GetSceneByBuildIndex(buildIndex);
        while (!scene.isLoaded)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(scene);
    }
    
    #endregion

    #region Loading Screen Utils

    private static IEnumerator CreateLoadingScreen()
    {
        s_Instance.m_CurrentLoadingScreen = Instantiate(s_Instance.m_LoadingScreenPrefab);
        SceneManager.MoveGameObjectToScene(currentLoadingScreen.gameObject, SceneManager.GetSceneByBuildIndex(Scenes.k_Persistent));
        yield return currentLoadingScreen.canvasBlender.Blend(true);
    }

    private static IEnumerator DestroyLoadingScreen()
    {
        yield return currentLoadingScreen.canvasBlender.Blend(false);
        Destroy(currentLoadingScreen.gameObject);
        s_Instance.m_CurrentLoadingScreen = null;
    }

    #endregion

    private IEnumerator Start()
    {
        yield return LoadMainMenuAtStartCoroutine();
    }

    private static IEnumerator LoadMainMenuAtStartCoroutine()
    {
        locked = true;
        yield return CreateLoadingScreen();
        camera.clearFlags = CameraClearFlags.Skybox;

        yield return LoadScene(Scenes.k_MainMenu, 0f, 1f);
        yield return SetActiveScene(Scenes.k_MainMenu);

        yield return DestroyLoadingScreen();
        locked = false;
    }

    private static void Quit(int exitCode = 0)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        if (exitCode != 0)
        {
            Debug.Log($"Program exited with exit code {exitCode}.");
        }
#else
        UnityEngine.Application.Quit(exitCode);
#endif
    }

    #region Main Menu Messages

    public static void OnClickPlay()
    {
        if (locked)
        {
            return;
        }

        s_Instance.StartCoroutine(ClickPlayCoroutine());
    }

    private static IEnumerator ClickPlayCoroutine()
    {
        locked = true;
        yield return CreateLoadingScreen();

        yield return UnloadScene(Scenes.k_MainMenu, 0.0f, 0.5f);
        yield return SetActiveScene(Scenes.k_Persistent);

        yield return LoadScene(Scenes.k_Debug, 0.5f, 1f);
        yield return SetActiveScene(Scenes.k_Debug);

        s_Instance.m_CurrentPauseScreen = Instantiate(s_Instance.m_PauseScreenPrefab);

        yield return DestroyLoadingScreen();
        locked = false;
    }

    public static void OnQuitFromMainMenu()
    {
        if (locked)
        {
            return;
        }

        s_Instance.StartCoroutine(QuitFromMainMenuCoroutine());
    }

    private static IEnumerator QuitFromMainMenuCoroutine()
    {
        locked = true;
        yield return CreateLoadingScreen();

        yield return UnloadScene(Scenes.k_MainMenu, 0f, 1f);
        yield return SetActiveScene(Scenes.k_Persistent);

        Quit();
        locked = false;
    }

    #endregion

    #region Pause Menu Messages

    public static void OnPause()
    {
        if (locked)
        {
            return;
        }

        if (currentPauseScreen == null)
        {
            return;
        }

        s_Instance.StartCoroutine(PauseCoroutine());
    }

    private static IEnumerator PauseCoroutine()
    {
        locked = true;
        Time.timeScale = 0f;
        yield return currentPauseScreen.Blend(true);
        locked = false;
    }

    public static void OnResume()
    {
        if (locked)
        {
            return;
        }

        if (currentPauseScreen == null)
        {
            return;
        }

        s_Instance.StartCoroutine(ResumeCoroutine());
    }

    private static IEnumerator ResumeCoroutine()
    {
        locked = true;
        yield return currentPauseScreen.Blend(false);
        Time.timeScale = 1f;
        locked = false;
    }

    public static void OnQuitToMainMenuFromDebugScene()
    {
        if (locked)
        {
            return;
        }

        s_Instance.StartCoroutine(QuitToMainMenuFromDebugSceneCoroutine());
    }

    private static IEnumerator QuitToMainMenuFromDebugSceneCoroutine()
    {
        locked = true;
        yield return CreateLoadingScreen();

        Destroy(currentPauseScreen);
        s_Instance.m_CurrentPauseScreen = null;

        Time.timeScale = 1f;

        yield return UnloadScene(Scenes.k_Debug, 0.0f, 0.5f);
        yield return SetActiveScene(Scenes.k_Persistent);

        s_Instance.m_CurrentPauseScreen = null;

        yield return LoadScene(Scenes.k_MainMenu, 0.5f, 1f);
        yield return SetActiveScene(Scenes.k_MainMenu);

        yield return DestroyLoadingScreen();
        locked = false;
    }

    public static void OnQuitFromDebugScene()
    {
        if (locked)
        {
            return;
        }

        s_Instance.StartCoroutine(QuitFromDebugSceneCoroutine());
    }

    private static IEnumerator QuitFromDebugSceneCoroutine()
    {
        locked = true;
        yield return CreateLoadingScreen();

        yield return UnloadScene(Scenes.k_Debug, 0f, 1f);
        yield return SetActiveScene(Scenes.k_Persistent);

        s_Instance.m_CurrentPauseScreen = null;

        Quit();
        locked = false;
    }

    #endregion
}