using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour
{
    [SerializeField] private GameObject agent = null;
    [SerializeField] private NavGraphNode nodePrefab = null;
    [SerializeField] private NavGraphEdge edgePrefab = null;

    [SerializeField] private float edgeLength = 5.0f;
    [SerializeField] private float graphRadius = 100.0f;
    [SerializeField] private float colliderCheckRadius = 0.3f;

    private NavGraph graph;

    private void Start()
    {
        graph = new NavGraph();
        if (agent == null) { return; }
        
        NavGraphNode nodeInstance = Instantiate(nodePrefab, agent.transform.position, agent.transform.rotation);
        nodeInstance.InitialiseNode(graph.GetIndexForNextNode(), agent.transform.position);
        graph.AddNode(nodeInstance);
        Flood(nodeInstance);
    }

    public NavGraph GetGraph()
    {
        if (graph != null)
        {
            return graph;
        }
        else
        {
            return null;
        }
    }

    private void Flood(NavGraphNode currNode)
    {
        if (Vector3.Distance(currNode.gameObject.transform.position, agent.transform.position) > graphRadius)
        {
            // return if we reached the graph radius
            return;
        }
        
        List<Vector3> positionsToSpawn = CalculateNextNodePositions(currNode);
        foreach (var pos in positionsToSpawn)
        {
            // Check if this position is inside a collider
            Collider[] colliders = Physics.OverlapSphere(pos, colliderCheckRadius);
            bool inObstacle = false;
            foreach (var collider in colliders)
            {
                if (collider.gameObject.tag == "Obstacle" || collider.gameObject.tag == "Wall")
                {
                    inObstacle = true;
                    break;
                }
            }
            if (inObstacle)
            {
                continue;
            }


            // Check if there is a node already at this position
            if (graph.NodePresentAtPosition(pos))
            {
                if (!graph.EdgeIsPresentBetween(currNode.transform.position, pos))
                {
                    // no edge, add one!
                    NavGraphEdge edge = CreateEdge(currNode, pos);
                    graph.AddEdge(edge);
                }
                continue;
            }

            NavGraphNode nodeInstance = Instantiate(nodePrefab, pos, agent.transform.rotation);
            nodeInstance.InitialiseNode(graph.GetIndexForNextNode(), pos);

            graph.AddNode(nodeInstance);

            NavGraphEdge edgeInstance = CreateEdge(currNode, pos);
            graph.AddEdge(edgeInstance);
            Flood(nodeInstance);
        }
    }

    private List<Vector3> CalculateNextNodePositions(NavGraphNode currNode)
    {
        List<Vector3> availablePositions = new List<Vector3>();
        for (int i = 0; i < 2; ++i)
        {
            var positivePos = new Vector3();
            var negativePos = new Vector3();
            var positiveDiagonalPos = new Vector3();
            var negativeDiagonalPos = new Vector3();
            switch (i)
            {
                case 0:
                    // x
                    positivePos = currNode.transform.position + new Vector3(edgeLength, 0, 0);
                    negativePos = currNode.transform.position - new Vector3(edgeLength, 0, 0);
                    positiveDiagonalPos = currNode.transform.position + new Vector3(edgeLength, 0, 0) + new Vector3(0, 0, edgeLength);
                    negativeDiagonalPos = currNode.transform.position - new Vector3(edgeLength, 0, 0) - new Vector3(0, 0, edgeLength);
                    break;
                case 1:
                    // z
                    positivePos = currNode.transform.position + new Vector3(0, 0, edgeLength);
                    negativePos = currNode.transform.position - new Vector3(0, 0, edgeLength);
                    positiveDiagonalPos = currNode.transform.position + new Vector3(edgeLength, 0, 0) - new Vector3(0, 0, edgeLength);
                    negativeDiagonalPos = currNode.transform.position - new Vector3(edgeLength, 0, 0) + new Vector3(0, 0, edgeLength);
                    break;
            }

            //Debug.Log(positivePos + " " + negativePos + " " + positiveDiagonalPos + " " + negativeDiagonalPos);
            availablePositions.Add(positivePos);
            availablePositions.Add(negativePos);
            availablePositions.Add(positiveDiagonalPos);
            availablePositions.Add(negativeDiagonalPos);
        }
        return availablePositions;
    }

    private NavGraphEdge CreateEdge(NavGraphNode currNode, Vector3 dest)
    {
        Vector3 diff = dest - currNode.transform.position;
        var edgeInstance = Instantiate(edgePrefab, (dest + currNode.transform.position) / 2, Quaternion.identity);
        edgeInstance.transform.eulerAngles += Quaternion.LookRotation(diff, Vector3.up).eulerAngles;
        edgeInstance.transform.Rotate(new Vector3(90, 0, 0));
        edgeInstance.InitialiseEdge(currNode.GetIndex(), graph.GetNodeAtPosition(dest).GetIndex(), diff.magnitude);
        return edgeInstance;
    }
}
