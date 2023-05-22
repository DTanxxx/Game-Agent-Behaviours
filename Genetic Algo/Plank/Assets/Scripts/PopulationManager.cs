using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public GameObject botPrefab;
    public int populationSize = 50;
    public float trialTime = 5;  // generation duration

    public static float elapsed = 0;  // amount of time elapsed

    private List<GameObject> population = new List<GameObject>();
    private int generation = 1;
    private GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;

        // display all relevant information for the process
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.EndGroup();
    }

    private void Start()
    {
        // spawn character prefabs
        for (int i = 0; i < populationSize; ++i)
        {
            Vector3 startingPos = new Vector3(transform.position.x + Random.Range(-2, 2),
                                              transform.position.y,
                                              transform.position.z + Random.Range(-2, 2));
            GameObject b = Instantiate(botPrefab, startingPos, transform.rotation);

            // initialize character's Brain component
            b.GetComponent<Brain>().Init();
            population.Add(b);
        }
    }

    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 startingPos = new Vector3(transform.position.x + Random.Range(-2, 2),
                                          transform.position.y,
                                          transform.position.z + Random.Range(-2, 2));
        GameObject offspring = Instantiate(botPrefab, startingPos, transform.rotation);
        Brain b = offspring.GetComponent<Brain>();

        if (Random.Range(0, 100) == 1)
        {
            // mutate with 1/100 probability
            b.Init();
            b.dna.Mutate();
        }
        else
        {
            // combine parents' DNAs
            b.Init();
            b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
        }
        return offspring;
    }

    private void BreedNewPopulation()
    {
        // sort population by timeAlive
        //List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain>().timeAlive).ToList();
        
        // sort population by distanceTravelled
        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain>().distanceTravelled).ToList();

        // clear the population list
        population.Clear();

        // breed upper half of sorted list
        for (int i = (int)(sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; ++i)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }

        // destroy all parents and previous population
        for (int i = 0; i < sortedList.Count; ++i)
        {
            Destroy(sortedList[i]);
        }

        generation += 1;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= trialTime)
        {
            // breed new generation
            BreedNewPopulation();
            elapsed = 0f;
        }
    }
}
