using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent
{
    public GameObject diamond;
    public GameObject painting;
    public GameObject van;
    public GameObject backDoor;
    public GameObject frontDoor;
    [Range(0, 1000)] public int money = 800;
    public GameObject[] art;
    public GameObject cop;

    private GameObject pickup;
    private Leaf goToFrontDoor;
    private Leaf goToBackDoor;
    private WaitForSeconds moneyWait;

    protected override void Start()
    {
        base.Start();

        // create a node for each action
        goToBackDoor = new Leaf("Go To Back Door", GoToBackDoor, 2);
        goToFrontDoor = new Leaf("Go To Front Door", GoToFrontDoor, 1);  // smaller sort order means priority selector will run this child first
        Leaf goToDiamond = new Leaf("Go To Diamond", GoToDiamond, 1);
        Leaf goToPainting = new Leaf("Go To Painting", GoToPainting, 2);
        Leaf goToVan = new Leaf("Go To Van", GoToVan);
        PrioritySelector openDoor = new PrioritySelector("Open Door", new GameObject[] { frontDoor, backDoor });
        RandomSelector selectObject = new RandomSelector("Select Object To Steal");
        Inverter invertMoney = new Inverter("Invert Money");
        Leaf hasGotMoney = new Leaf("Has Got Money?", HasMoney);
        Inverter invertGalleryOpen = new Inverter("Gallery Closed");
        Leaf isGalleryOpen = new Leaf("Is Gallery Open?", IsGalleryOpen);

        // the inverter will invert the status returned by its child
        invertMoney.AddChild(hasGotMoney);

        invertGalleryOpen.AddChild(isGalleryOpen);

        // the selector will choose a door to enter into the gallery
        openDoor.AddChild(goToFrontDoor);
        openDoor.AddChild(goToBackDoor);

        // the selector will choose an asset to steal
        for (int i = 0; i < art.Length; ++i)
        {
            // add a leaf for each stealable art object
            Leaf goToArt = new Leaf("Go to " + art[i].name, GoToArt, i);
            selectObject.AddChild(goToArt);
        }

        Sequence runAway = new Sequence("Run Away");
        Leaf canSee = new Leaf("Can See Cop", CanSeeCop);
        Leaf flee = new Leaf("Flee From Cop", FleeFromCop);

        runAway.AddChild(canSee);
        runAway.AddChild(flee);

        Inverter cannotSeeCop = new Inverter("Cannot See Cop");
        cannotSeeCop.AddChild(canSee);

        // make a dependency tree
        BehaviourTree stealConditions = new BehaviourTree();
        Sequence conditions = new Sequence("Steal Conditions");
        conditions.AddChild(cannotSeeCop);
        conditions.AddChild(invertMoney);
        conditions.AddChild(invertGalleryOpen);
        stealConditions.AddChild(conditions);

        DependencySequence steal = new DependencySequence("Steal Something", stealConditions, agent);
        steal.AddChild(openDoor);
        steal.AddChild(selectObject);
        steal.AddChild(goToVan);

        // add a fallback behaviour - goToVan
        Selector stealWithFallback = new Selector("Steal with Fallback");
        stealWithFallback.AddChild(steal);
        stealWithFallback.AddChild(goToVan);

        Selector rob = new Selector("Rob");
        rob.AddChild(stealWithFallback);
        rob.AddChild(runAway);
        tree.AddChild(rob);

        // NOTE THAT THE ORDER OF CHILD NODES MATTERS

        tree.PrintTree();

        moneyWait = new WaitForSeconds(Random.Range(1, 5));
        StartCoroutine(DecreaseMoney());
    }

    private IEnumerator DecreaseMoney()
    {
        while (true)
        {
            money = Mathf.Clamp(money - 50, 0, 1000);
            yield return moneyWait;
        }
    }

    public Node.Status CanSeeCop()
    {
        return CanSee(cop.transform.position, "Cop", 10f, 90f);
    }

    public Node.Status FleeFromCop()
    {
        return Flee(cop.transform.position, 10f);
    }

    public Node.Status HasMoney()
    {
        if (money < 500)
        {
            // does not have enough money, go steal
            return Node.Status.FAILURE;
        }
        return Node.Status.SUCCESS;
    }

    public Node.Status GoToBackDoor()
    {
        Node.Status status = GoToDoor(backDoor);
        if (status == Node.Status.FAILURE)
        {
            // child failed, lower its priority so next time BTree runs priority selector, this child will be chosen last
            goToBackDoor.sortOrder = 10;
        }
        else
        {
            // raise its priority so next time BTree runs priority selector, this child will be chosen first
            goToBackDoor.sortOrder = 1;
        }
        return status;
    }

    public Node.Status GoToFrontDoor()
    {
        Node.Status status = GoToDoor(frontDoor);
        if (status == Node.Status.FAILURE)
        {
            goToFrontDoor.sortOrder = 10;
        }
        else
        {
            goToFrontDoor.sortOrder = 1;
        }
        return status;   
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
            // attach diamond to robber if robber gets to diamond successfully
            art[index].transform.parent = transform;
            pickup = art[index];
        }
        return status;
    }

    public Node.Status GoToDiamond()
    {
        if (!diamond.activeSelf)
        {
            return Node.Status.FAILURE;
        }

        Node.Status status = GoToLocation(diamond.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            // attach diamond to robber if robber gets to diamond successfully
            diamond.transform.parent = transform;
            pickup = diamond;
        }
        return status;
    }

    public Node.Status GoToPainting()
    {
        if (!painting.activeSelf)
        {
            return Node.Status.FAILURE;
        }

        Node.Status status = GoToLocation(painting.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            // attach diamond to robber if robber gets to diamond successfully
            painting.transform.parent = transform;
            pickup = painting;
        }
        return status;
    }

    public Node.Status GoToVan()
    {
        Node.Status status = GoToLocation(van.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            // add money only if robber has an item
            if (pickup != null)
            {
                money += 300;
                pickup.SetActive(false);
                pickup = null;
            }
        }
        return status;
    }
}
