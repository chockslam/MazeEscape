using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PrimsAlgorithm : MazeAlgorithm
{
    public PrimsAlgorithm(int depth, int width, Cell[,] grid) : base(depth, width, grid)
    {
    }

    public override IEnumerator GenerateMaze(float generationSpeedInSeconds)
    {
        List<Cell> frontierCells = new List<Cell>();
        Cell currentCell = m_grid[0, 0];
        currentCell.Visit();
        AddFrontierCells(currentCell, frontierCells);

        while (frontierCells.Count > 0)
        {
            Cell nextCell = ChooseFrontierCell(frontierCells); // Implement selection logic
            Cell adjacentCell = GetVisitedNeighbor(nextCell); // Find an already visited neighbor
            RemoveWallBetween(adjacentCell, nextCell);
            nextCell.Visit();
            AddFrontierCells(nextCell, frontierCells);
            yield return new WaitForSeconds(generationSpeedInSeconds); // Visualize step
        }
    }

    Cell ChooseFrontierCell(List<Cell> frontierCells)
    {
        if (frontierCells.Count == 0) return null;
        int index = UnityEngine.Random.Range(0, frontierCells.Count);
        Cell chosenCell = frontierCells[index];
        frontierCells.RemoveAt(index); // Remove the chosen cell from the frontier list
        return chosenCell;
    }

    Cell GetVisitedNeighbor(Cell frontierCell)
    {
        int x = Mathf.RoundToInt(frontierCell.transform.position.x);
        int z = Mathf.RoundToInt(frontierCell.transform.position.z);

        List<Cell> visitedNeighbors = new List<Cell>();

        // Check each direction for visited neighbors
        CheckAndAddVisitedNeighbor(x - 1, z, visitedNeighbors); // Left
        CheckAndAddVisitedNeighbor(x + 1, z, visitedNeighbors); // Right
        CheckAndAddVisitedNeighbor(x, z - 1, visitedNeighbors); // Back
        CheckAndAddVisitedNeighbor(x, z + 1, visitedNeighbors); // Front

        if (visitedNeighbors.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, visitedNeighbors.Count);
            return visitedNeighbors[randomIndex];
        }

        return null; // Should never happen if used correctly
    }

    void CheckAndAddVisitedNeighbor(int x, int z, List<Cell> visitedNeighbors)
    {
        if (x >= 0 && x < m_width && z >= 0 && z < m_depth && m_grid[x, z].m_isVisited)
        {
            visitedNeighbors.Add(m_grid[x, z]);
        }
    }

    void AddFrontierCells(Cell currentCell, List<Cell> frontierCells)
    {
        int x = Mathf.RoundToInt(currentCell.transform.position.x);
        int z = Mathf.RoundToInt(currentCell.transform.position.z);

        // Add unvisited neighbors to the frontier list
        AddIfValid(x - 1, z, frontierCells); // Left
        AddIfValid(x + 1, z, frontierCells); // Right
        AddIfValid(x, z - 1, frontierCells); // Back
        AddIfValid(x, z + 1, frontierCells); // Front
    }

    void AddIfValid(int x, int z, List<Cell> frontierCells)
    {
        if (x >= 0 && x < m_width && z >= 0 && z < m_depth)
        {
            Cell potentialFrontierCell = m_grid[x, z];
            if (!potentialFrontierCell.m_isVisited && !frontierCells.Contains(potentialFrontierCell))
            {
                frontierCells.Add(potentialFrontierCell);
            }
        }
    }


}