using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMonitor : MonoBehaviour
{
    public string state;
    public float stateStrength;  //  how long a given state can persist before a resource spawns
    public float stateDecayRate;  // state countdown
    public WorldStates beliefs;
    public GameObject resourcePrefab;
    public string queueName;
    public string worldState;
    public GameAction action;  // this action must be carried out to NOT produce the resource

    private bool stateFound = false;
    private float initialStrength;

    private void Start()
    {
        beliefs = GetComponent<GameAgent>().beliefs;
        initialStrength = stateStrength;
    }

    private void LateUpdate()
    {
        if (action.running)
        {
            // action is running, this monitor shouldn't change anything
            stateFound = false;
            stateStrength = initialStrength;
            return;
        }

        if (!stateFound && beliefs.HasState(state))
        {
            stateFound = true;
        }

        if (stateFound)
        {
            stateStrength -= stateDecayRate * Time.deltaTime;
            if (stateStrength <= 0f)
            {
                // produce a resource at the agent's feet
                Vector3 location = new Vector3(transform.position.x, 
                    resourcePrefab.transform.position.y,
                    transform.position.z);
                GameObject obj = Instantiate(resourcePrefab, location, resourcePrefab.transform.rotation);
                stateFound = false;
                stateStrength = initialStrength;
                beliefs.RemoveState(state);  // remove belief state once the resource is produced (as resource production is triggered by that belief state)
                GameWorld.Instance.GetQueue(queueName).AddResource(obj);
                GameWorld.Instance.GetWorld().ModifyState(worldState, 1);
            }
        }
    }
}
