using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Pool;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField]
    MazeAlgorithmType selectedAlgorithm = MazeAlgorithmType.DepthFirstSearch;

    [SerializeField]
    MazeGenerationSettings m_mazeSettings;

    ObjectPooler m_objectPooler;
    Cell[,] m_grid;
    IMazeAlgorithm m_mazeAlgorithm;

    public IEnumerator Initialize(ObjectPooler objectPooler)
    {
        m_objectPooler = objectPooler;
        InitializeGrid();
        InitializeAlgorithm();
        yield return StartCoroutine(m_mazeAlgorithm.GenerateMaze(m_mazeSettings.generationSpeedInSeconds));
    }

    public IEnumerator RegenerateMaze()
    {
        StopAllCoroutines();
        foreach (Transform child in transform)
        {
            m_objectPooler.ReturnObjectToPool(PoolTag.Cell, child.gameObject);
        }

        InitializeGrid();
        InitializeAlgorithm();
        yield return m_mazeAlgorithm.GenerateMaze(m_mazeSettings.generationSpeedInSeconds);
    }

    public Vector2Int GetStartPosition() 
    {
        return m_mazeSettings.startCellPosition;
    }

    public Vector2Int GetEndPosition()
    {
        return m_mazeSettings.endCellPosition;
    }


    public Vector2Int FindRandomSpawnPosition()
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        for (int x = 0; x < m_mazeSettings.width; x++)
        {
            for (int z = 0; z < m_mazeSettings.depth; z++)
            {
                Vector2Int potentialPosition = new Vector2Int(x, z);
                Cell currentCell = m_grid[x, z];

                if (potentialPosition != m_mazeSettings.startCellPosition && potentialPosition != m_mazeSettings.endCellPosition && !currentCell.IsOccupied())
                {
                    validPositions.Add(potentialPosition);
                }
            }
        }

        if (validPositions.Count == 0)
        {
            throw new System.Exception("No valid spawn positions available.");
        }

        int randomIndex = UnityEngine.Random.Range(0, validPositions.Count);
        Vector2Int selectedPosition = validPositions[randomIndex];

        m_grid[selectedPosition.x, selectedPosition.y].Occupy();

        return selectedPosition;
    }

    void InitializeGrid()
    {
        m_grid = new Cell[m_mazeSettings.width, m_mazeSettings.depth];
        for (int x = 0; x < m_mazeSettings.width; ++x)
        {
            for (int z = 0; z < m_mazeSettings.depth; ++z)
            {
                GameObject obj = m_objectPooler.GetPooledObject(PoolTag.Cell, new Vector3(x, 0, z), Quaternion.identity, transform);
                Cell cell = obj.GetComponent<Cell>();
                if (cell != null)
                {
                    m_grid[x, z] = cell;
                }
                else
                {
                    Debug.LogError("Spawned object does not have a Cell component.");
                }
            }
        }
    }

    void InitializeAlgorithm()
    {
        switch (selectedAlgorithm)
        {
            case MazeAlgorithmType.DepthFirstSearch:
                m_mazeAlgorithm = new DepthFirstSearch(m_mazeSettings.depth, m_mazeSettings.width, m_grid);
                break;
            case MazeAlgorithmType.PrimsAlgorithm:
                m_mazeAlgorithm = new PrimsAlgorithm(m_mazeSettings.depth, m_mazeSettings.width, m_grid);
                break;
            case MazeAlgorithmType.KruskalsAlgorithm:
                m_mazeAlgorithm = new KruskalsAlgorithm(m_mazeSettings.depth, m_mazeSettings.width, m_grid);
                break;
        }

        m_mazeAlgorithm.SetStartAndEndCells(m_mazeSettings.startCellPosition, m_mazeSettings.endCellPosition);
    }

    public Cell[,] GetGrid() 
    {
        return m_grid;
    }
}
