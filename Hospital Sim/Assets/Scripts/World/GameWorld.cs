using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResourceQueue
{
    public Queue<GameObject> que = new Queue<GameObject>();
    public string tag;
    public string modState;

    public ResourceQueue(string t, string ms, WorldStates worldStates)
    {
        tag = t;
        modState = ms;
        if (tag != "")
        {
            // if we have a tag, populate que with corresponding resources
            GameObject[] resources = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in resources)
            {
                que.Enqueue(obj);
            }
        }

        if (modState != "")
        {
            // if we have a modState string, then modify the corresponding world state
            worldStates.ModifyState(modState, que.Count);
        }
    }

    public void AddResource(GameObject obj)
    {
        que.Enqueue(obj);
    }

    public GameObject RemoveResource()
    {
        if (que.Count == 0)
        {
            return null;
        }
        return que.Dequeue();
    }

    public void RemoveResource(GameObject obj)
    {
        que = new Queue<GameObject>(que.Where(p => p != obj));
    }
}

public sealed class GameWorld
{
    private static readonly GameWorld instance = new GameWorld();  // a singleton
    private static WorldStates world;
    private static ResourceQueue patients;  // patients will register themselves to "patients"
    private static ResourceQueue cubicles;
    private static ResourceQueue offices;
    private static ResourceQueue toilets;
    private static ResourceQueue puddles;
    private static Dictionary<string, ResourceQueue> resources = new Dictionary<string, ResourceQueue>();

    static GameWorld()
    {
        world = new WorldStates();
        patients = new ResourceQueue("", "", world);
        cubicles = new ResourceQueue("Cubicle", "freeCubicle", world);
        offices = new ResourceQueue("Office", "freeOffice", world);
        toilets = new ResourceQueue("Toilet", "freeToilet", world);
        puddles = new ResourceQueue("Puddle", "freePuddle", world);
        
        resources.Add("patients", patients);
        resources.Add("cubicles", cubicles);
        resources.Add("offices", offices);
        resources.Add("toilets", toilets);
        resources.Add("puddles", puddles);

        Time.timeScale = 5;
    }

    public ResourceQueue GetQueue(string type)
    {
        return resources[type];
    }

    private GameWorld()
    {

    }

    public static GameWorld Instance
    {
        get
        {
            return instance;
        }
    }

    public WorldStates GetWorld()
    {
        return world;
    }
}
