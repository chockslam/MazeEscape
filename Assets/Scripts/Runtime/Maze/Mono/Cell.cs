using System;
using UnityEngine;

public class Cell : MonoBehaviour, IPoolable
{
    [SerializeField]
    public GameObject go_wallLeft;

    [SerializeField]
    public GameObject go_wallRight;

    [SerializeField]
    public GameObject go_wallFront;

    [SerializeField]
    public GameObject go_wallBack;

    [SerializeField]
    GameObject go_visitedBlock;

    [SerializeField]
    Material startCellMaterial;

    [SerializeField]
    Material endCellMaterial;

    [NonSerialized]
    public bool m_isVisited = false;

    private bool m_isOccupied = false;

    public void Visit()
    {
        go_visitedBlock.SetActive(false);
        m_isVisited = true;
    }

    public void DestroyWall(CellEdge edge)
    {
        switch (edge)
        {
            case CellEdge.Left:
                _DestroyWall(go_wallLeft);
                break;
            case CellEdge.Right:
                _DestroyWall(go_wallRight);
                break;
            case CellEdge.Front:
                _DestroyWall(go_wallFront);
                break;
            case CellEdge.Back:
                _DestroyWall(go_wallBack);
                break;
            default:
                //Log
                break;
        }
    }

    // TODO: Make a smaller gap to enter?!
    private void _DestroyWall(GameObject wall)
    {
        wall.SetActive(false);
    }

    public void MarkAsStart()
    {
        if (startCellMaterial != null)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.material = startCellMaterial;
            }
        }
    }

    public void MarkAsEnd()
    {
        if (endCellMaterial != null)
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.material = endCellMaterial;
            }
        }
    }

    public bool IsOccupied() 
    {
        return m_isOccupied;
    }

    public void Occupy() 
    {
        m_isOccupied = true;
    }

    public void OnObjectSpawn()
    {
        go_wallLeft.SetActive(true);
        go_wallRight.SetActive(true);
        go_wallFront.SetActive(true);
        go_wallBack.SetActive(true);

        m_isVisited = false;
        go_visitedBlock.SetActive(true);
    }

    public void OnObjectReturn()
    {
    }

    public enum CellEdge
    {
        Left,
        Right,
        Front,
        Back
    }
}
