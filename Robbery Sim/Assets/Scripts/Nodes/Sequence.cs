using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(string n)
    {
        name = n;
    }

    public override Status Process()
    {
        Debug.Log("Sequence: " + name + " " + currentChild);
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
