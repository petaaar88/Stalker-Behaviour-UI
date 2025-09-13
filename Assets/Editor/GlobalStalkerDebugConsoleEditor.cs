using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalStalkerDebugConsole))]
public class GlobalStalkerDebugConsoleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var console = (GlobalStalkerDebugConsole)target;
        var names = console.GetStalkerNames();

        if (names.Length > 0)
        {
            console.selectedStalkerIndex = EditorGUILayout.Popup(
                "Select Stalker",
                console.selectedStalkerIndex,
                names
            );
        }

        DrawDefaultInspector();
    }
}
