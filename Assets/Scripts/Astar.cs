using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    public const int maxLoops = 100000;

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> openNodes = new List<Node>();
        openNodes.Add(new Node(startPos,null,0,Vector2Int.Distance(startPos, endPos)));
        List<Node> closedNodes = new List<Node>();
        int loops = 0;
        Dictionary<Vector2Int, Wall> directionToDirection = new Dictionary<Vector2Int, Wall>()
        {
            { new Vector2Int(0, 1), Wall.UP },
            { new Vector2Int(0, -1), Wall.DOWN },
            { new Vector2Int(1, 0), Wall.RIGHT },
            { new Vector2Int(-1, 0), Wall.LEFT }
        };
        while (openNodes.Count > 0 && loops < maxLoops)
        {
            //For stats and to prevent an infintie loop
            loops++;
            
            //Ordering and preperation
            openNodes = openNodes.OrderBy(x => x.FScore).ToList();
            Node current = openNodes[0];
            openNodes.Remove(current);
            closedNodes.Add(current);
            
            //Is our goal reached?
            if (current.position == endPos)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                Node currentBack = current;
                while (currentBack != null)
                {
                    path.Add(currentBack.position);
                    currentBack = currentBack.parent;
                }
                Debug.Log("Objective found in " + loops + " loops!");
                path.Reverse();
                return path;
            }
            
            //Get adjacent nodes of current node
            Cell currentCell = grid[current.position.x,current.position.y];
            List<Cell> neighbors = currentCell.GetNeighbours(grid);
            List<Node> adjacent = new List<Node>();
            foreach (var neighbor in neighbors)
                if (!currentCell.HasWall(directionToDirection[neighbor.gridPosition - currentCell.gridPosition]))
                    adjacent.Add(new Node(neighbor.gridPosition, current, 0, 0));
            
            //Process those nodes
            foreach (var node in adjacent)
            {
                //Node already visited
                if (closedNodes.Any(x => x.EqualTo(node)))
                    continue;
                
                //Calculate the scores
                node.GScore = current.GScore + Vector2Int.Distance(node.position, current.position);
                node.HScore = Vector2Int.Distance(node.position, endPos);
                
                //If the node is already open
                if (openNodes.Any(x => x.EqualTo(node)))
                {
                    //Find the one in the list
                    Node inList = openNodes.Where(x => x.EqualTo(node)).ToList()[0];
                    if (inList.GScore < node.GScore) //See if the new one is faster
                    {
                        openNodes.Remove(inList);
                        openNodes.Add(node);
                    }
                }
                else
                {
                    //Add it to the unvisited nodes to process
                    openNodes.Add(node);
                }
            }
        }
        return null;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, float GScore, float HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }

        public bool EqualTo(Node a) => a.position == position;
    }
}
