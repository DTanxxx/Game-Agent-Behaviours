using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
    public delegate Status Tick();
    public Tick ProcessMethod;  // function pointer, pointing to a function that returns Status

    public delegate Status TickMulti(int index);
    public TickMulti MultiProcessMethod;

    public int index;

    public Leaf() { }

    public Leaf(string n, Tick method)
    {
        name = n;
        ProcessMethod = method;
    }

    public Leaf(string n, Tick method, int order)
    {
        name = n;
        ProcessMethod = method;
        sortOrder = order;
    }

    public Leaf(string n, TickMulti method, int i)
    {
        name = n;
        MultiProcessMethod = method;
        index = i;
    }

    public override Status Process()
    {
        Node.Status status;
        if (ProcessMethod != null)
        {
            status = ProcessMethod();
        }
        else if (MultiProcessMethod != null)
        {
            status = MultiProcessMethod(index);
        }
        else
        {
            status = Status.FAILURE;
        }

        Debug.Log(name + " " + status);
        return status;
    }
}
