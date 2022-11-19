using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Essentials.Core.CheatConsole
{
    public static class CheatConsoleGameObject
    {
        [MenuItem("GameObject/Essentials/Cheat Console", false, 2)]
        private static void CreateConsole()
        {
            GameObject consolePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.notrewd.essentials/EssentialsCore/Prefabs/CheatConsole/ConsoleCanvas.prefab");
            GameObject instance = Object.Instantiate(consolePrefab, Selection.activeGameObject != null ? Selection.activeGameObject.transform : null);
            instance.name = "ConsoleCanvas";
            Selection.activeGameObject = instance;
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}