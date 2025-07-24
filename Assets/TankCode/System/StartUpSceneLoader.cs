using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace TankCode.System
{
    [InitializeOnLoad]
    public static class StartUpSceneLoader
    {
        static StartUpSceneLoader()
        {
            EditorApplication.playModeStateChanged += HandlePlayModeChange;
        }

        private static void HandlePlayModeChange(PlayModeStateChange changeState)
        {
            if (changeState == PlayModeStateChange.ExitingEditMode)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }

            if (changeState == PlayModeStateChange.EnteredEditMode)
            {
                if (SceneManager.GetActiveScene().buildIndex != 0)
                {
                    SceneManager.LoadScene(0);
                }
            }
        }
    }
}