using System.Collections;
using UnityEngine;

public interface IMazeAlgorithm
{
    IEnumerator GenerateMaze(float generationSpeedInSeconds);
    void SetStartAndEndCells(Vector2Int start, Vector2Int end);
}
