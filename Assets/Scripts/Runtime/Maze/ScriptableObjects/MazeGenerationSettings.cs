using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MazeGenerationSettings", menuName = "Maze/Settings", order = 1)]
public class MazeGenerationSettings : ScriptableObject
{
    public Cell cellPrefab;
    public int width = 10;
    public int depth = 10;
    public float generationSpeedInSeconds = 0.1f;
    public Vector2Int endCellPosition = new() { x = 0, y = 0 };
    public Vector2Int startCellPosition = new() { x = 9, y = 9 };
}
