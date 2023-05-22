using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class GameAction : MonoBehaviour
{
    public string actionName = "Action";
    public float cost = 1.0f;  // planner will find the cheapest plan using those costs
    public GameObject target;  // destination
    public string targetTag;
    public float duration = 0.0f;  // action duration
    public WorldState[] preConditions;
    public WorldState[] afterEffects;
    public NavMeshAgent agent;

    public Dictionary<string, int> preconditions;
    public Dictionary<string, int> effects;

    public WorldStates agentBeliefs;
    public GameInventory inventory;
    public WorldStates beliefs;

    public bool running = false;

    public GameAction()
    {
        preconditions = new Dictionary<string, int>();
        effects = new Dictionary<string, int>();
    }

    public void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (preConditions != null)
        {
            foreach (WorldState worldState in preConditions)
            {
                preconditions.Add(worldState.key, worldState.value);
            }
        }

        if (afterEffects != null)
        {
            foreach (WorldState worldState in afterEffects)
            {
                effects.Add(worldState.key, worldState.value);
            }
        }

        inventory = GetComponent<GameAgent>().inventory;
        beliefs = GetComponent<GameAgent>().beliefs;
    }

    public bool IsAchievable()
    {
        return true;
    }

    public bool IsAchievableGiven(Dictionary<string, int> conditions)
    {
        foreach (KeyValuePair<string, int> pair in preconditions)
        {
            // "conditions" must contain every condition in preconditions
            if (!conditions.ContainsKey(pair.Key))
            {
                return false;
            }
        }
        return true;
    }

    public abstract bool PrePerform();
    public abstract bool PostPerform();
}
