using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KruskalsAlgorithm : MazeAlgorithm
{
    public KruskalsAlgorithm(int depth, int width, Cell[,] grid) : base(depth, width, grid)
    {
    }

    public override IEnumerator GenerateMaze(float generationSpeedInSeconds)
    {
        List<MazeEdge> edges = GetAllPossibleEdges();
        UnionFind unionFind = new UnionFind(m_width * m_depth);

        while (edges.Count > 0)
        {
            int randomIndex = Random.Range(0, edges.Count);
            MazeEdge randomEdge = edges[randomIndex];
            edges.RemoveAt(randomIndex);

            Cell cellA = randomEdge.CellA;
            Cell cellB = randomEdge.CellB;

            int indexA = CellToIndex(cellA);
            int indexB = CellToIndex(cellB);

            if (unionFind.Find(indexA) != unionFind.Find(indexB))
            {
                unionFind.Union(indexA, indexB);
                RemoveWallBetween(cellA, cellB);
                cellA.Visit();
                cellB.Visit(); 
                yield return new WaitForSeconds(generationSpeedInSeconds);
            }
        }
    }

    List<MazeEdge> GetAllPossibleEdges()
    {
        List<MazeEdge> edges = new List<MazeEdge>();
        for (int x = 0; x < m_width; x++)
        {
            for (int z = 0; z < m_depth; z++)
            {
                if (x > 0) edges.Add(new MazeEdge(m_grid[x, z], m_grid[x - 1, z]));
                if (z > 0) edges.Add(new MazeEdge(m_grid[x, z], m_grid[x, z - 1]));
                if (x < m_width - 1) edges.Add(new MazeEdge(m_grid[x, z], m_grid[x + 1, z]));
                if (z < m_depth - 1) edges.Add(new MazeEdge(m_grid[x, z], m_grid[x, z + 1]));
            }
        }
        return edges;
    }

    int CellToIndex(Cell cell)
    {
        int x = Mathf.RoundToInt(cell.transform.position.x);
        int z = Mathf.RoundToInt(cell.transform.position.z);
        return x + z * m_width;
    }
}

class UnionFind
{
    private int[] parent;

    public UnionFind(int size)
    {
        parent = new int[size];
        for (int i = 0; i < size; i++)
        {
            parent[i] = i;
        }
    }

    public int Find(int x)
    {
        if (parent[x] != x) parent[x] = Find(parent[x]);
        return parent[x];
    }

    public void Union(int x, int y)
    {
        int rootX = Find(x);
        int rootY = Find(y);
        if (rootX != rootY)
        {
            parent[rootY] = rootX;
        }
    }
}

struct MazeEdge
{
    public Cell CellA;
    public Cell CellB;

    public MazeEdge(Cell cellA, Cell cellB)
    {
        CellA = cellA;
        CellB = cellB;
    }
}
