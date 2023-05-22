using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldState
{
    public string key;
    public int value;
}

public class WorldStates
{
    public Dictionary<string, int> states;

    public WorldStates()
    {
        states = new Dictionary<string, int>();
    }

    public bool HasState(string key)
    {
        return states.ContainsKey(key);
    }

    public void AddState(string key, int value)
    {
        states.Add(key, value);
    }

    public void ModifyState(string key, int value)
    {
        if (states.ContainsKey(key))
        {
            // already added the state to dictionary, increase its value by "value"
            states[key] += value;
            if (states[key] <= 0)
            {
                // remove the state indexed by key
                RemoveState(key);
            }
        }
        else
        {
            states.Add(key, value);
        }
    }

    public void RemoveState(string key)
    {
        if (states.ContainsKey(key))
        {
            states.Remove(key);
        }
    }

    public void SetState(string key, int amount)
    {
        if (states.ContainsKey(key))
        {
            states[key] = amount;
        }
        else
        {
            states.Add(key, amount);
        }
    }

    public Dictionary<string, int> GetStates()
    {
        // used by planner to get all world states
        return states;
    }
}
