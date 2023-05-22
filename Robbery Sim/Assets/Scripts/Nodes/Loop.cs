using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : Node
{
    private BehaviourTree dependency;

    public Loop(string n, BehaviourTree dependencyTree)
    {
        name = n;
        dependency = dependencyTree;
    }

    public override Status Process()
    {
        Status dependencyStatus = dependency.Process();
        if (dependencyStatus == Status.FAILURE)
        {
            // dependency fails, we don't need this sequence to execute anymore, return successful to move onto next actions
            return Status.SUCCESS;
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
            return childStatus;
        }

        currentChild += 1;
        if (currentChild >= children.Count)
        {
            // loop around child nodes, because as long as dependency returns true, we want to continue executing this sequence
            currentChild = 0;
        }

        return Status.RUNNING;
    }
}
