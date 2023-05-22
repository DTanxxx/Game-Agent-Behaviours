using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGraphEdge : MonoBehaviour
{
    private int sourceNodeIndex;
    private int destinationNodeIndex;
    private double cost;

    public void InitialiseEdge(int source, int destination, double cost)
    {
        this.sourceNodeIndex = source;
        this.destinationNodeIndex = destination;
        this.cost = cost;
    }

    public int GetSourceNodeIndex()
    {
        return sourceNodeIndex;
    }

    public void SetSourceNode(int index)
    {
        this.sourceNodeIndex = index;
    }

    public int GetDestinationNodeIndex()
    {
        return destinationNodeIndex;
    }

    public void SetDestinationNode(int index)
    {
        this.destinationNodeIndex = index;
    }

    public double GetCost()
    {
        return cost;
    }

    public void SetCost(double cost)
    {
        this.cost = cost;
    }
}
