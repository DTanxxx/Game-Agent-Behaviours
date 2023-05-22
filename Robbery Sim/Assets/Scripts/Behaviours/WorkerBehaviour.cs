using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerBehaviour : BTAgent
{
    public GameObject office;
    
    private GameObject targetPatron;

    protected override void Start()
    {
        base.Start();

        Leaf goToPatron = new Leaf(name + "Go To Patron", GoToPatron);
        Leaf goToOffice = new Leaf(name + "Go To Office", GoToOffice);
        Leaf allocatePatron = new Leaf(name + "Allocate Patron", AllocatePatron);
        Leaf patronStillWaiting = new Leaf(name + "Is Patron Waiting?", PatronWaiting);

        Sequence getPatron = new Sequence(name + "Find A Patron");
        getPatron.AddChild(allocatePatron);

        BehaviourTree waiting = new BehaviourTree();
        waiting.AddChild(patronStillWaiting);

        DependencySequence moveToPatron = new DependencySequence("Moving To Patron", waiting, agent);
        moveToPatron.AddChild(goToPatron);

        getPatron.AddChild(moveToPatron);

        Selector beWorker = new Selector("Be A Worker");
        beWorker.AddChild(getPatron);
        beWorker.AddChild(goToOffice);

        tree.AddChild(beWorker);
    }

    public Node.Status AllocatePatron()
    {
        // grab a patron in the stack
        if (Blackboard.Instance.patrons.Count == 0)
        {
            // we have no patron to give ticket to
            return Node.Status.FAILURE;
        }
        targetPatron = Blackboard.Instance.patrons.Pop();
        if (targetPatron == null)
        {
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToPatron()
    {
        if (targetPatron == null)
        {
            // no patron allocated
            return Node.Status.FAILURE;
        }

        Node.Status status = GoToLocation(targetPatron.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            // reached a patron, give them ticket
            targetPatron.GetComponent<PatronBehaviour>().hasTicket = true;
            targetPatron = null;
        }
        return status;
    }

    public Node.Status GoToOffice()
    {
        Node.Status status = GoToLocation(office.transform.position);
        targetPatron = null;
        return status;
    }

    public Node.Status PatronWaiting()
    {
        if (targetPatron == null)
        {
            // no assigned patron, worker shouldn't do anything
            return Node.Status.FAILURE;
        }

        if (targetPatron.GetComponent<PatronBehaviour>().isWaiting)
        {
            // worker has a waiting patron
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }
}
