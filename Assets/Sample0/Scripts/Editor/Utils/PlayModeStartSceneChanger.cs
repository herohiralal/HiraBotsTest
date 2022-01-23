using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace AIEngineTest.Editor
{
    [InitializeOnLoad]
    internal static class PlayModeStartSceneChanger
    {
        static PlayModeStartSceneChanger()
        {
            EditorSceneManager.playModeStartScene = null;
            // s_NumberOfPersistentScenesOnLastUpdate = -1;
            // EditorApplication.update -= OnEditorUpdate;
            // EditorApplication.update += OnEditorUpdate;
        }

        private static int s_NumberOfPersistentScenesOnLastUpdate;

        private static void OnEditorUpdate()
        {
            var persistentSceneGuids = AssetDatabase.FindAssets("_PERSISTENT t:scene");

            if (persistentSceneGuids == null || persistentSceneGuids.Length == 0)
            {
                if (s_NumberOfPersistentScenesOnLastUpdate != 0)
                {
                    Debug.LogError("Could not find any persistent scenes in the project.");
                }

                s_NumberOfPersistentScenesOnLastUpdate = 0;
                return;
            }

            if (persistentSceneGuids.Length != 1)
            {
                if (s_NumberOfPersistentScenesOnLastUpdate != persistentSceneGuids.Length)
                {
                    var sb = new StringBuilder(
                        "Multiple persistent scenes have been detected. The project must have only one persistent scene.\n" +
                        "\n" +
                        "They are as follows:\n",
                        10000);

                    foreach (var sceneGuid in persistentSceneGuids)
                    {
                        sb.AppendLine(AssetDatabase.GUIDToAssetPath(sceneGuid));
                    }

                    Debug.LogError(sb.ToString());
                }

                s_NumberOfPersistentScenesOnLastUpdate = persistentSceneGuids.Length;
                return;
            }

            var persistentScenePath = AssetDatabase.GUIDToAssetPath(persistentSceneGuids[0]);
            var persistentSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(persistentScenePath);
            EditorSceneManager.playModeStartScene = persistentSceneAsset;
            EditorApplication.update -= OnEditorUpdate;
        }
    }
}