using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(string n)
    {
        name = n;
    }

    public override Status Process()
    {
        Status childStatus = children[currentChild].Process();
        if (childStatus == Status.RUNNING)
        {
            return Status.RUNNING;
        }
        if (childStatus == Status.SUCCESS)
        {
            // as soon as a child succeeds, this whole sequence succeeds
            currentChild = 0;
            return childStatus;
        }

        // move onto next child
        currentChild += 1;
        if (currentChild >= children.Count)
        {
            // finished running all children
            currentChild = 0;
            return Status.FAILURE;
        }

        // this sequence is still running
        return Status.RUNNING;
    }
}
