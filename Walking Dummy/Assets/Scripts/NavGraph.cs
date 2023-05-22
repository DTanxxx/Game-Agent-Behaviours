using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGraph
{
    private List<NavGraphNode> nodeList = new List<NavGraphNode>();
    private List<List<NavGraphEdge>> edgeList = new List<List<NavGraphEdge>>();

    private Dictionary<Vector3, int> nodePosHashToIndexMap = new Dictionary<Vector3, int>();

    private int indexForNextNode = 1;

    public int GetIndexForNextNode()
    {
        return indexForNextNode;
    }

    public NavGraphNode GetNode(int index)
    {
        return nodeList[index-1];
    }

    public NavGraphNode GetNodeAtPosition(Vector3 pos)
    {
        return GetNode(nodePosHashToIndexMap[pos]);
    }

    public NavGraphEdge GetEdge(int from, int to)
    {
        return edgeList[from][to];
    }

    public int AddNode(NavGraphNode node)
    {
        nodeList.Add(node);
        indexForNextNode++;
        nodePosHashToIndexMap.Add(node.GetPosition(), node.GetIndex());
        return node.GetIndex();
    }

    public void RemoveNode(int index)
    {
        nodePosHashToIndexMap[nodeList[index - 1].GetPosition()] = -1;
        nodeList[index-1] = null;
    }

    public void AddEdge(NavGraphEdge edge)
    {
        while (edgeList.Count <= edge.GetSourceNodeIndex())
        {
            // number of rows in edge list is too little
            edgeList.Add(new List<NavGraphEdge>());
        }
        while (edgeList[edge.GetSourceNodeIndex()].Count <= edge.GetDestinationNodeIndex())
        {
            // number of columns in the sub list is too little
            edgeList[edge.GetSourceNodeIndex()].Add(null);
        }
        edgeList[edge.GetSourceNodeIndex()][edge.GetDestinationNodeIndex()] = edge;
    }

    public void RemoveEdge(int from, int to)
    {
        edgeList[from][to] = null;
    }

    public int GetTotalNumNodes()
    {
        return nodeList.Count;
    }

    public int GetNumActiveNodes()
    {
        int count = 0;
        foreach (NavGraphNode node in nodeList)
        {
            if (node != null)
            {
                count++;
            }
        }
        return count;
    }

    public int GetTotalNumEdges()
    {
        int count = 0;
        for (int i=0; i<edgeList.Count; ++i)
        {
            for (int j=0; j<edgeList[i].Count; ++j)
            {
                if (edgeList[i][j] != null)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public bool IsEmpty()
    {
        return (nodeList.Count == 0);
    }

    public bool NodeIsPresent(int index)
    {
        if (index > nodeList.Count || nodeList[index-1] == null)
        {
            return false;
        }
        return true;
    }

    public bool EdgeIsPresentBetween(Vector3 source, Vector3 dest)
    {
        int sourceIndex = GetNodeAtPosition(source).GetIndex();
        int destIndex = GetNodeAtPosition(dest).GetIndex();
        if ((sourceIndex < edgeList.Count && destIndex < edgeList[sourceIndex].Count && edgeList[sourceIndex][destIndex] != null) 
            || (destIndex < edgeList.Count && sourceIndex < edgeList[destIndex].Count && edgeList[destIndex][sourceIndex]))
        {
            // there is an edge
            return true;
        }
        return false;
    }

    public void Clear()
    {
        nodeList.Clear();
        edgeList.Clear();
        indexForNextNode = 0;
    }

    /// <summary>
    /// Searches the nodePosHashToIndexMap for pos's hash
    /// </summary>
    /// <param name="pos"></param>
    /// <returns> True if dictionary already has this node's position </returns>
    public bool NodePresentAtPosition(Vector3 pos)
    {
        return nodePosHashToIndexMap.ContainsKey(pos);
    }
}
