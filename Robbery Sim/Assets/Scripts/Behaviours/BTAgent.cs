using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTAgent : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected BehaviourTree tree;
    protected ActionState state = ActionState.IDLE;
    protected Node.Status treeStatus = Node.Status.RUNNING;

    private WaitForSeconds waitForSeconds;
    
    protected Vector3 rememberedLocation;

    public enum ActionState { IDLE, WORKING };

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        tree = new BehaviourTree();
        waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 1.0f));
        StartCoroutine(Behave());  // potential race condition with RobberBehaviour's Start()
    }

    private IEnumerator Behave()
    {
        while (true)
        {
            treeStatus = tree.Process();
            yield return waitForSeconds;
        }
    }

    protected Node.Status GoToLocation(Vector3 destination)
    {
        float distanceToTarget = Vector3.Distance(destination, transform.position);
        if (state == ActionState.IDLE)
        {
            // agent is idle, give it a destination to travel to
            agent.SetDestination(destination);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, destination) >= 2f)
        {
            // agent did not make to destination, fail this action
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distanceToTarget < 2f)
        {
            // agent is close to destination, succeed
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }

    protected Node.Status CanSee(Vector3 target, string tag, float maxDistance, float maxAngle)
    {
        Vector3 directionToTarget = target - transform.position;
        float angle = Vector3.Angle(directionToTarget, transform.forward);

        if (angle <= maxAngle || directionToTarget.magnitude <= maxDistance)
        {
            // if target is close or within field of view angle, perform a raycast
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToTarget, out hit))
            {
                // if raycast hits an object with tag "tag", return success
                if (hit.collider.gameObject.CompareTag(tag))
                {
                    return Node.Status.SUCCESS;
                }
            }
        }
        return Node.Status.FAILURE;
    }

    protected Node.Status Flee(Vector3 fleeFrom, float runDistance)
    {
        // run away to a location opposite to fleeFrom's position
        if (state == ActionState.IDLE)
        {
            // set a destination location when robber is not currently in action
            rememberedLocation = transform.position + (transform.position - fleeFrom).normalized * runDistance;
        }
        return GoToLocation(rememberedLocation);
    }

    protected Node.Status GoToDoor(GameObject door)
    {
        Node.Status status = GoToLocation(door.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            // agent has made to the door (front or back)
            if (!door.GetComponent<Lock>().isLocked)
            {
                // disable the unlocked door's navmeshobstacle once agent reaches it
                door.GetComponent<NavMeshObstacle>().enabled = false;
                return Node.Status.SUCCESS;
            }
            // if door is locked, fail this action and let selector move on to the next action
            return Node.Status.FAILURE;
        }
        else
        {
            return status;  // RUNNING or FAILURE, return either way
        }
    }

    protected Node.Status IsGalleryOpen()
    {
        if (Blackboard.Instance.timeOfDay < Blackboard.Instance.openTime ||
            Blackboard.Instance.timeOfDay > Blackboard.Instance.closeTime)
        {
            // not opened
            return Node.Status.FAILURE;
        }
        else
        {
            // gallery open for visit
            return Node.Status.SUCCESS;
        }
    }
}
