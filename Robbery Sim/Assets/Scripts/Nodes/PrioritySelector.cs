using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PrioritySelector : Node
{
    private Node[] nodeArray;
    private GameObject[] objects;
    private bool sorted = false;

    public PrioritySelector(string n, GameObject[] objs)
    {
        name = n;
        objects = objs;
    }

    public override Status Process()
    {
        if (!sorted)
        {
            OrderChildren();
            sorted = true;
        }

        Status childStatus = children[currentChild].Process();
        if (childStatus == Status.RUNNING)
        {
            return Status.RUNNING;
        }
        if (childStatus == Status.SUCCESS)
        {
            // as soon as a child succeeds, this whole sequence succeeds
            currentChild = 0;
            sorted = false;
            return childStatus;
        }

        // move onto next child
        currentChild += 1;
        if (currentChild >= children.Count)
        {
            // finished running all children
            currentChild = 0;
            sorted = false;
            return Status.FAILURE;
        }

        // this sequence is still running
        return Status.RUNNING;
    }

    private void OrderChildren()
    {
        nodeArray = children.ToArray();
        Sort(nodeArray, 0, children.Count - 1);
        children = new List<Node>(nodeArray);
    }

    //QuickSort
    //Adapted from: https://exceptionnotfound.net/quick-sort-csharp-the-sorting-algorithm-family-reunion/
    int Partition(Node[] array, int low,
                                int high)
    {
        Node pivot = array[high];

        int lowIndex = (low - 1);

        //2. Reorder the collection.
        for (int j = low; j < high; j++)
        {
            if (array[j].sortOrder <= pivot.sortOrder)
            {
                lowIndex++;

                Node temp = array[lowIndex];
                array[lowIndex] = array[j];
                array[j] = temp;
            }
        }

        Node temp1 = array[lowIndex + 1];
        array[lowIndex + 1] = array[high];
        array[high] = temp1;

        return lowIndex + 1;
    }

    void Sort(Node[] array, int low, int high)
    {
        if (low < high)
        {
            int partitionIndex = Partition(array, low, high);
            Sort(array, low, partitionIndex - 1);
            Sort(array, partitionIndex + 1, high);
        }
    }
}
