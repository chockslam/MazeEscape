using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameManager))]
public class MazeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager gameManager = (GameManager)target;

        if (Application.isPlaying)
        {
            if (gameManager.IsLoading)
            {
                EditorGUILayout.HelpBox("Cannot regenrate while loading", MessageType.Warning);
            }
            else if (GUILayout.Button("Regenerate Maze"))
            {
                gameManager.RestartLevel();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Maze regeneration is available only in Play Mode.", MessageType.Info);
        }
    }
}
