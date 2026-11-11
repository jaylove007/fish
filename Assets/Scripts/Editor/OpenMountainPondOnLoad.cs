using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fish.Prototype.EditorTools
{
    /// <summary>
    /// A fresh clone has no Library, so Unity opens an untitled scene.
    /// Load the pond once the editor is idle, and leave any real scene alone.
    /// </summary>
    [InitializeOnLoad]
    static class OpenMountainPondOnLoad
    {
        const string ScenePath = "Assets/Scenes/MountainPond.unity";
        static int _tries;

        static OpenMountainPondOnLoad()
        {
            EditorApplication.delayCall += TryOpen;
        }

        static void TryOpen()
        {
            if (++_tries > 300)
                return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += TryOpen;
                return;
            }

            if (Application.isBatchMode || EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            var scene = SceneManager.GetActiveScene();
            if (scene.path == ScenePath || !string.IsNullOrEmpty(scene.path) || scene.isDirty)
                return;

            if (!File.Exists(ScenePath))
                return;

            EditorSceneManager.OpenScene(ScenePath);
        }
    }
}
