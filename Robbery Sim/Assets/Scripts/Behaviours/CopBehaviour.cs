using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopBehaviour : BTAgent
{
    public GameObject[] patrolPoints;
    public GameObject robber;

    protected override void Start()
    {
        base.Start();

        Sequence selectPatrolPoint = new Sequence("Select Patrol Point");
        for (int i = 0; i < patrolPoints.Length; ++i)
        {
            Leaf patrolPoint = new Leaf("Go To " + patrolPoints[i].name, GoToPoint, i);
            selectPatrolPoint.AddChild(patrolPoint);
        }

        Sequence chaseRobber = new Sequence("Chase Robber");
        Leaf canSeeRobber = new Leaf("Can See Robber", CanSeeRobber);
        Leaf chase = new Leaf("Chase Robber", ChaseRobber);

        chaseRobber.AddChild(canSeeRobber);
        chaseRobber.AddChild(chase);

        Inverter cannotSeeRobber = new Inverter("Cannot See Robber");
        cannotSeeRobber.AddChild(canSeeRobber);

        BehaviourTree patrolConditions = new BehaviourTree();
        Sequence condition = new Sequence("Patrol Conditions");
        condition.AddChild(cannotSeeRobber);
        patrolConditions.AddChild(condition);

        DependencySequence patrol = new DependencySequence("Patrol Until", patrolConditions, agent);
        patrol.AddChild(selectPatrolPoint);

        Selector beCop = new Selector("Be A Cop");
        beCop.AddChild(patrol);
        beCop.AddChild(chaseRobber);

        tree.AddChild(beCop);
    }

    private Node.Status GoToPoint(int i)
    {
        Node.Status status = GoToLocation(patrolPoints[i].transform.position);
        return status;
    }

    public Node.Status CanSeeRobber()
    {
        return CanSee(robber.transform.position, "Robber", 5f, 60f);
    }

    public Node.Status ChaseRobber()
    {
        float chaseDistance = 10f;
        if (state == ActionState.IDLE)
        {
            // set a destination location when cop is not currently in action
            rememberedLocation = transform.position - (transform.position - robber.transform.position).normalized * chaseDistance;
        }
        return GoToLocation(rememberedLocation);
    }
}
