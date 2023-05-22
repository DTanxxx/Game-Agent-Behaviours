using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatronBehaviour : BTAgent
{
    public GameObject[] art;
    public GameObject frontDoor;
    public GameObject home;
    [Range(0, 1000)] public int boredom = 0;
    public bool hasTicket = false;
    public bool isWaiting = false;

    private WaitForSeconds boredomWait;

    protected override void Start()
    {
        base.Start();

        RandomSelector selectObject = new RandomSelector(name + "Select Art To View");
        for (int i = 0; i < art.Length; ++i)
        {
            // add a leaf for each stealable art object
            Leaf goToArt = new Leaf(name + "Go to " + art[i].name, GoToArt, i);
            selectObject.AddChild(goToArt);
        }

        Leaf goToFrontDoor = new Leaf(name + "Go To Frontdoor", GoToFrontDoor);
        Leaf goToHome = new Leaf(name + "Go Home", GoToHome);
        Leaf isBored = new Leaf(name + "Is Bored?", IsBored);
        Leaf isGalleryOpen = new Leaf(name + "Is Gallery Open?", IsGalleryOpen);

        Sequence viewArt = new Sequence(name + "View Art");
        viewArt.AddChild(isGalleryOpen);
        viewArt.AddChild(isBored);
        viewArt.AddChild(goToFrontDoor);

        Leaf noTicket = new Leaf(name + "Wait For Ticket", NoTicket);
        Leaf isWaiting = new Leaf(name + "Waiting For Worker", IsWaiting);

        BehaviourTree waitForTicket = new BehaviourTree();
        waitForTicket.AddChild(noTicket);

        // as long as patron has no ticket, keep executing IsWaiting action
        Loop getTicket = new Loop(name + "Ticket", waitForTicket);
        getTicket.AddChild(isWaiting);

        viewArt.AddChild(getTicket);

        BehaviourTree whileBored = new BehaviourTree();
        whileBored.AddChild(isBored);

        Loop lookAtPaintings = new Loop(name + "Look", whileBored);
        lookAtPaintings.AddChild(selectObject);  // while bored, keep selecting art to visit

        viewArt.AddChild(lookAtPaintings);
        viewArt.AddChild(goToHome);

        BehaviourTree galleryOpenCondition = new BehaviourTree();
        galleryOpenCondition.AddChild(isGalleryOpen);

        DependencySequence bePatron = new DependencySequence(name + "Be An Art Patron", galleryOpenCondition, agent);
        bePatron.AddChild(viewArt);

        Selector viewArtWithFallback = new Selector(name + "View Art With Fallback");
        viewArtWithFallback.AddChild(bePatron);
        viewArtWithFallback.AddChild(goToHome);

        tree.AddChild(viewArtWithFallback);

        boredomWait = new WaitForSeconds(Random.Range(1, 5));
        StartCoroutine(IncreaseBoredom());
    }

    private IEnumerator IncreaseBoredom()
    {
        while (true)
        {
            boredom = Mathf.Clamp(boredom + 10, 0, 1000);
            yield return boredomWait;
        }
    }

    public Node.Status GoToArt(int index)
    {
        if (!art[index].activeSelf)
        {
            return Node.Status.FAILURE;
        }
        Node.Status status = GoToLocation(art[index].transform.position);
        if (status == Node.Status.SUCCESS)
        {
            // reduce boredom if visit art is successful
            boredom = Mathf.Clamp(boredom - 150, 0, 1000);
        }
        return status;
    }

    public Node.Status GoToFrontDoor()
    {
        Node.Status status = GoToDoor(frontDoor);
        return status;
    }

    public Node.Status GoToHome()
    {
        Node.Status status = GoToLocation(home.transform.position);
        isWaiting = false;
        return status;
    }

    public Node.Status IsBored()
    {
        if (boredom < 100)
        {
            // not bored
            return Node.Status.FAILURE;
        }
        else
        {
            // bored, go to front door and visit art
            return Node.Status.SUCCESS;
        }
    }

    public Node.Status NoTicket()
    {
        if (hasTicket || IsGalleryOpen() == Node.Status.FAILURE)
        {
            // stop loop from running if patron has a ticket or if gallery is closed
            return Node.Status.FAILURE;
        }
        else
        {
            // no ticket and gallery is open
            return Node.Status.SUCCESS;
        }
    }

    public Node.Status IsWaiting()
    {
        if (Blackboard.Instance.RegisterPatron(gameObject))
        {
            // this patron has registered, it is now waiting
            isWaiting = true;
            return Node.Status.SUCCESS;
        }
        else
        {
            return Node.Status.FAILURE;
        }
    }
}
