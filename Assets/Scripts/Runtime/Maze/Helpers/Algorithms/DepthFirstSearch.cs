using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirstSearch : MazeAlgorithm
{
    public DepthFirstSearch(int depth, int width, Cell[,] grid) : base(depth, width, grid)
    {
    }

    public override IEnumerator GenerateMaze(float generationSpeedInSeconds)
    {
        Stack<Cell> stack = new();
        Cell currentCell = m_grid[0, 0];
        currentCell.Visit();
        stack.Push(currentCell);

        while (stack.Count > 0)
        {
            Cell nextCell = GetUnvisitedNeighborDFS(currentCell);
            if (nextCell != null)
            {
                stack.Push(currentCell);
                RemoveWallBetween(currentCell, nextCell);
                currentCell = nextCell;
                currentCell.Visit();
                yield return new WaitForSeconds(generationSpeedInSeconds); // Visualize step
            }
            else if (stack.Count > 0)
            {
                currentCell = stack.Pop();
            }
        }
    }

    Cell GetUnvisitedNeighborDFS(Cell currentCell)
    {

        int x = Mathf.RoundToInt(currentCell.transform.position.x);
        int z = Mathf.RoundToInt(currentCell.transform.position.z);

        List<Cell> unvisitedNeighbors = new();

        if (x > 0 && !m_grid[x - 1, z].m_isVisited)
            unvisitedNeighbors.Add(m_grid[x - 1, z]);
        

        if (x < m_width - 1 && !m_grid[x + 1, z].m_isVisited)
            unvisitedNeighbors.Add(m_grid[x + 1, z]);
        

        if (z > 0 && !m_grid[x, z - 1].m_isVisited)
            unvisitedNeighbors.Add(m_grid[x, z - 1]);
        

        if (z < m_depth - 1 && !m_grid[x, z + 1].m_isVisited)
            unvisitedNeighbors.Add(m_grid[x, z + 1]);
        

        if (unvisitedNeighbors.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, unvisitedNeighbors.Count);
            return unvisitedNeighbors[randomIndex];
        }

        return null; // No unvisited neighbors
    }
}