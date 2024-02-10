using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class MazeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager GameManager = (GameManager)target;

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Regenerate Maze"))
            {
                GameManager.RestartLevel();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Maze regeneration is available only in Play Mode.", MessageType.Info);
        }
    }
}
