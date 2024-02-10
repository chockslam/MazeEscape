using System.Collections;
using UnityEngine;

public abstract class MazeAlgorithm : IMazeAlgorithm
{
    protected Cell[,] m_grid;
    protected Cell startCell, endCell;
    protected int m_depth, m_width;

    protected MazeAlgorithm(int depth, int width, Cell[,] grid)
    {
        m_depth = depth;
        m_grid = grid;
        m_width = width;
    }

    public abstract IEnumerator GenerateMaze(float generationSpeedInSeconds);

    protected void RemoveWallBetween(Cell cellA, Cell cellB)
    {
        int ax = Mathf.RoundToInt(cellA.transform.position.x);
        int az = Mathf.RoundToInt(cellA.transform.position.z);
        int bx = Mathf.RoundToInt(cellB.transform.position.x);
        int bz = Mathf.RoundToInt(cellB.transform.position.z);

        if (ax == bx)
        {
            if (az > bz)
            {
                cellA.DestroyWall(Cell.CellEdge.Back);
                cellB.DestroyWall(Cell.CellEdge.Front);
            }
            else
            {
                cellA.DestroyWall(Cell.CellEdge.Front);
                cellB.DestroyWall(Cell.CellEdge.Back);
            }
        }
        else if (az == bz)
        {
            if (ax > bx)
            {
                cellA.DestroyWall(Cell.CellEdge.Left);
                cellB.DestroyWall(Cell.CellEdge.Right);
            }
            else
            {
                cellA.DestroyWall(Cell.CellEdge.Right);
                cellB.DestroyWall(Cell.CellEdge.Left);
            }
        }
    }

    public void SetStartAndEndCells(Vector2Int start, Vector2Int end)
    {
        startCell = m_grid[start.x, start.y];
        startCell.MarkAsStart();

        endCell = m_grid[end.x, end.y];
        endCell.MarkAsEnd();
    }
}