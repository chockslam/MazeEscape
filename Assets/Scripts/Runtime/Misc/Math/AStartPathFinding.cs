using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathfinding
{
    private class Node
    {
        public Vector3 gridPosition;
        public Node parent;
        public int gCost;
        public int hCost;
        public int FCost => gCost + hCost;

        public Node(Vector3 pos, Node parentNode, int g, int h)
        {
            gridPosition = pos;
            parent = parentNode;
            gCost = g;
            hCost = h;
        }
    }

    public List<Vector3> FindPath(Cell[,] grid, Vector3 start, Vector3 end)
    {

        start = new Vector3(
            Mathf.Round(start.x),
            0.0f,
            Mathf.Round(start.z)
        );

        end = new Vector3(
            Mathf.Round(end.x),
            0.0f,
            Mathf.Round(end.z)
        );

        List<Node> openList = new();
        HashSet<Node> closedList = new HashSet<Node>();
        Node startNode = new((Vector3)start, null, 0, MathHelper.GetManhattanDistance(start, end));

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost && openList[i].hCost < currentNode.hCost))
                    currentNode = openList[i];
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode.gridPosition == end)
            {
                return RetracePath(startNode, currentNode);
            }

            foreach (Vector3 neighbour in GetNeighbours(grid, currentNode.gridPosition))
            {
                if (closedList.Any(n => n.gridPosition == neighbour)) continue;

                int newGCost = currentNode.gCost + MathHelper.GetManhattanDistance(currentNode.gridPosition, neighbour);
                Node neighbourNode = openList.FirstOrDefault(n => n.gridPosition == neighbour);
                if (neighbourNode == null)
                {
                    neighbourNode = new Node(neighbour, currentNode, newGCost, MathHelper.GetManhattanDistance(neighbour, end));
                    openList.Add(neighbourNode);
                }
                else if (newGCost < neighbourNode.gCost)
                {
                    neighbourNode.gCost = newGCost;
                    neighbourNode.parent = currentNode;
                }
            }
        }

        return new List<Vector3>();
    }

    List<Vector3> RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.gridPosition);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    List<Vector3> GetNeighbours(Cell[,] grid, Vector3 gridPosition)
    {
        List<Vector3> neighbours = new List<Vector3>();
        int x = Mathf.RoundToInt(gridPosition.x);
        int z = Mathf.RoundToInt(gridPosition.z);

        if (x > 0 && !grid[x, z].go_wallLeft.activeSelf) 
            neighbours.Add(new Vector3(x - 1, 0, z));
        if (x < grid.GetLength(0) - 1 && !grid[x, z].go_wallRight.activeSelf) 
            neighbours.Add(new Vector3(x + 1, 0, z));
        if (z > 0 && !grid[x, z].go_wallBack.activeSelf)
            neighbours.Add(new Vector3(x, 0, z - 1));
        if (z < grid.GetLength(1) - 1 && !grid[x, z].go_wallFront.activeSelf) 
            neighbours.Add(new Vector3(x, 0, z + 1));

        return neighbours;
    }

}
