using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DependencySequence : Node
{
    private BehaviourTree dependency;
    private NavMeshAgent agent;

    public DependencySequence(string n, BehaviourTree bt, NavMeshAgent agent)
    {
        name = n;
        dependency = bt;
        this.agent = agent;
    }

    public override Status Process()
    {
        // if the dependency tree fails, terminate this tree
        Status dependencyStatus = dependency.Process();
        if (dependencyStatus == Status.FAILURE)
        {
            agent.ResetPath();

            // reset all children
            currentChild = 0;
            foreach (Node n in children)
            {
                n.Reset();
            }
            return Status.FAILURE;
        }
        else if (dependencyStatus == Status.RUNNING)
        {
            // if dependency tree is still running, we mark this sequence as running too
            // this way we prevent this sequence from executing until all dependency nodes are checked
            return Status.RUNNING;
        }

        Status childStatus = children[currentChild].Process();
        if (childStatus == Status.RUNNING)
        {
            return Status.RUNNING;
        }
        if (childStatus == Status.FAILURE)
        {
            // as soon as a child fails, this whole sequence fails
            currentChild = 0;
            foreach (Node n in children)
            {
                n.Reset();
            }
            return childStatus;
        }

        // move onto next child
        currentChild += 1;
        if (currentChild >= children.Count)
        {
            // finished running all children
            currentChild = 0;
            return Status.SUCCESS;
        }

        // this sequence is still running
        return Status.RUNNING;
    }
}
