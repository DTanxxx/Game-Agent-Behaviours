using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    private List<GameObject> population = new List<GameObject>();
    private int generation = 1;
    private GUIStyle guiStyle = new GUIStyle();

    public GameObject botPrefab;
    public int populationSize = 50;
    public float trialTime = 5;

    public static float elapsed = 0;

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.EndGroup();
    }

    // use this for initialization
    private void Start()
    {
        for (int i = 0; i < populationSize; ++i)
        {
            Vector3 startingPos = new Vector3(transform.position.x + Random.Range(-2, 2),
                transform.position.y, transform.position.z + Random.Range(-2, 2));
            GameObject obj = Instantiate(botPrefab, startingPos, transform.rotation);
            obj.GetComponent<Brain>().Init();
            population.Add(obj);
        }
    }

    private GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 startingPos = new Vector3(transform.position.x + Random.Range(-2, 2),
            transform.position.y, transform.position.z + Random.Range(-2, 2));
        GameObject offspring = Instantiate(botPrefab, startingPos, transform.rotation);
        Brain b = offspring.GetComponent<Brain>();
        if (Random.Range(0, 100) == 1)
        {
            // mutate 1 in 100 chance
            b.Init();
            b.dna.Mutate();
        }
        else
        {
            b.Init();
            b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
        }
        return offspring;
    }

    private void BreedNewPopulation()
    {
        // give timeWalking some weight so more agents will be walking around rather than spinning around in a spot
        List<GameObject> sortedList = population.OrderBy(o => (o.GetComponent<Brain>().timeAlive + o.GetComponent<Brain>().timeWalking * 5)).ToList();

        population.Clear();

        // split population in half, and breed the upper half (fittest individuals)
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
            BreedNewPopulation();
            elapsed = 0;
        }
    }
}
