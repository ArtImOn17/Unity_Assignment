using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

[InitializeOnLoad]
public static class AssignmentInputRepair
{
    static AssignmentInputRepair()
    {
        EditorApplication.delayCall += Repair;
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.EnteredEditMode)
                EditorApplication.delayCall += Repair;
        };
    }

    [MenuItem("Assignment/Repair input")]
    public static void Repair()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.path != "Assets/Scenes/Platformer.unity") return;
        var actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Input/PlatformerInput.inputactions");
        if (actions == null)
        {
            Debug.LogError("Input repair: PlatformerInput has not imported yet. Run Assignment > Repair input after import completes.");
            return;
        }
        bool changed = false;
        bool wasDirty = scene.isDirty;
        foreach (var root in scene.GetRootGameObjects())
        foreach (var input in root.GetComponentsInChildren<PlayerInput>(true))
        {
            if (input.actions != null) continue;
            var serialized = new SerializedObject(input);
            serialized.FindProperty("m_Actions").objectReferenceValue = actions;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(input);
            changed = true;
        }
        if (!changed) return;
        EditorSceneManager.MarkSceneDirty(scene);
        if (!wasDirty) EditorSceneManager.SaveScene(scene);
        Debug.Log("Input repaired: PlatformerInput assigned to PlayerInput.");
    }
}
