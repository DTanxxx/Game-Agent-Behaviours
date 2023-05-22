using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SubGoal
{
    public Dictionary<string, int> subgoals;
    public bool remove;  // subgoal gets removed if it is done

    public SubGoal(string s, int i, bool r)
    {
        subgoals = new Dictionary<string, int>();
        subgoals.Add(s, i);
        remove = r;
    }
}

public class GameAgent : MonoBehaviour
{
    public List<GameAction> actions = new List<GameAction>();
    public Dictionary<SubGoal, int> goals = new Dictionary<SubGoal, int>();
    public GameInventory inventory = new GameInventory();
    public WorldStates beliefs = new WorldStates();

    public GameAction currentAction;

    private GamePlanner planner;
    private Queue<GameAction> actionQueue;
    private SubGoal currentGoal;
    private bool invoked = false;
    private Vector3 destination = Vector3.zero;

    protected virtual void Start()
    {
        GameAction[] acts = GetComponents<GameAction>();
        foreach (GameAction a in acts)
        {
            actions.Add(a);
        }
    }

    private void CompleteAction()
    {
        currentAction.running = false;
        currentAction.PostPerform();
        invoked = false;  // reset "invoked" so LateUpdate can finish other actions
    }

    private void LateUpdate()
    {
        if (currentAction != null && currentAction.running)
        {
            // agent is running an action
            float distanceToTarget = Vector3.Distance(destination, transform.position);
            if (distanceToTarget < 2f)
            {
                // agent has reached its goal, complete currentAction
                if (!invoked)
                {
                    Invoke("CompleteAction", currentAction.duration);
                    invoked = true;
                }
            }
            return;
        }

        if (planner == null || actionQueue == null)
        {
            // agent has no plan, create one
            planner = new GamePlanner();

            // sort through all goals from most to least important (goal.value)
            var sortedGoals = from entry in goals orderby entry.Value descending select entry;
            
            // plan a sequence of actions for each subgoal
            foreach (KeyValuePair<SubGoal, int> subgoal in sortedGoals)
            {
                actionQueue = planner.Plan(actions, subgoal.Key.subgoals, beliefs);
                if (actionQueue != null)
                {
                    // set the most important and achievable goal as currentGoal
                    currentGoal = subgoal.Key;
                    break;
                }
            }
        }

        if (actionQueue != null && actionQueue.Count == 0)
        {
            // agent has ran out of actions to take, plan is finished
            if (currentGoal.remove)
            {
                goals.Remove(currentGoal);
            }
            planner = null;
        }

        if (actionQueue != null && actionQueue.Count > 0)
        {
            // agent still has actions to do
            currentAction = actionQueue.Dequeue();
            if (currentAction.PrePerform())
            {
                if (currentAction.target == null && currentAction.targetTag != "")
                {
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);
                }

                if (currentAction.target != null)
                {
                    // start this action
                    currentAction.running = true;

                    // find target's destination
                    destination = currentAction.target.transform.position;
                    Transform destTransform = currentAction.target.transform.Find("Destination");
                    if (destTransform != null)
                    {
                        // found a custom-made gameobject for destination
                        destination = destTransform.position;
                    }

                    currentAction.agent.SetDestination(destination);
                }
            }
            else
            {
                // action fails, replan
                actionQueue = null;
            }
        }
    }
}
