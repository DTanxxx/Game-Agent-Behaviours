using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Node parent;
    public float cost;
    public Dictionary<string, int> state;
    public GameAction action;  // the action this node is pointing to

    // this constructor only worries about world states
    public Node(Node parent, float cost, Dictionary<string, int> allStates, GameAction gameAction)
    {
        this.parent = parent;
        this.cost = cost;
        state = new Dictionary<string, int>(allStates);  // a copy of allStates dict
        action = gameAction;
    }

    // overloaded constructor taking in belief states as well
    public Node(Node parent, float cost, Dictionary<string, int> allStates, Dictionary<string, int> beliefStates, GameAction gameAction)
    {
        this.parent = parent;
        this.cost = cost;
        state = new Dictionary<string, int>(allStates);  // a copy of allStates dict
        
        // add in belief states too
        foreach (KeyValuePair<string, int> pair in beliefStates)
        {
            if (!state.ContainsKey(pair.Key))
            {
                state.Add(pair.Key, pair.Value);
            }
        }
        action = gameAction;
    }
}

public class GamePlanner
{
    public Queue<GameAction> Plan(List<GameAction> actions, Dictionary<string, int> goal, WorldStates beliefStates)
    {
        List<GameAction> usableActions = new List<GameAction>();
        
        // filter out unusable actions
        foreach (GameAction a in actions)
        {
            if (a.IsAchievable())
            {
                usableActions.Add(a);
            }
        }

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0f, GameWorld.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
            // no plan found in graph
            Debug.Log("NO PLAN");
            return null;
        }

        // found a plan that we can follow
        // now find the cheapest leaf node
        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null)
            {
                cheapest = leaf;
            }
            else if (leaf.cost < cheapest.cost)
            {
                cheapest = leaf;
            }
        }

        List<GameAction> result = new List<GameAction>();  // a sequence of actions

        // construct a path of actions starting from "cheapest" back to root node
        Node n = cheapest;
        while (n != null)
        {
            if (n.action != null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        // finally, construct a queue out of the action sequence we just found
        Queue<GameAction> queue = new Queue<GameAction>();
        foreach (GameAction a in result)
        {
            queue.Enqueue(a);
        }

        Debug.Log("The plan is: ");
        foreach (GameAction a in queue)
        {
            Debug.Log("Q: " + a.actionName);
        }

        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<GameAction> usableActions, Dictionary<string, int> goal) 
    {
        bool foundPath = false;
        foreach (GameAction action in usableActions)
        {
            if (action.IsAchievableGiven(parent.state))
            {
                // this action can be carried out given parent's world state
                // create a new world state based off parent's world state
                Dictionary<string, int> currentState = new Dictionary<string, int>(parent.state);
                foreach (KeyValuePair<string, int> effect in action.effects)
                {
                    // if currentState does not have the effects from action.effects, add them
                    if (!currentState.ContainsKey(effect.Key))
                    {
                        currentState.Add(effect.Key, effect.Value);
                    }
                }

                // create a child node
                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                // check if "node" achieves the goal
                if (GoalAchieved(goal, currentState))
                {
                    // add to the leaves list
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    // extract a subset of usableActions (exclude "action") and pass them in to recursive call
                    List<GameAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                    {
                        foundPath = true;
                    }
                }
            }
        }
        return foundPath;
    }

    private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
    {
        // check if each condition in "goal" is satisfied in "state"
        foreach (KeyValuePair<string, int> pair in goal)
        {
            if (!state.ContainsKey(pair.Key))
            {
                return false;
            }
        }
        return true;
    }

    private List<GameAction> ActionSubset(List<GameAction> actions, GameAction removeMe)
    {
        List<GameAction> subset = new List<GameAction>();
        foreach (GameAction action in actions)
        {
            if (!action.Equals(removeMe))
            {
                subset.Add(action);
            }
        }
        return subset;
    }
}
