using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public GameObject personPrefab;
    public int populationSize = 10;

    public static float elapsed = 0f;

    private List<GameObject> population = new List<GameObject>();
    private int trialTime = 10;  // each session is 10 seconds
    private int generation = 1;

    private GUIStyle guiStyle = new GUIStyle();

    private void Start()
    {
        for (int i = 0; i < populationSize; ++i)
        {
            Vector3 pos = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);
            GameObject obj = Instantiate(personPrefab, pos, Quaternion.identity);
            obj.GetComponent<DNA>().r = Random.Range(0f, 1f);
            obj.GetComponent<DNA>().g = Random.Range(0f, 1f);
            obj.GetComponent<DNA>().b = Random.Range(0f, 1f);
            obj.GetComponent<DNA>().s = Random.Range(0.1f, 0.3f);
            population.Add(obj);
        }
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > trialTime)
        {
            // one generation is finished
            BreedNewPopulation();
            elapsed = 0;
        }
    }

    private void BreedNewPopulation()
    {
        List<GameObject> newPopulation = new List<GameObject>();

        // remove unfit individuals
        List<GameObject> sortedList = population.OrderBy(obj => obj.GetComponent<DNA>().timeToDie).ToList();
        population.Clear();

        // breed lower half of sorted list (this is how it's sorted)
        // the larger the timeToDie value, the more likely their genes will be passed on
        for (int i = (int)(sortedList.Count / 2.0f) - 1; i < sortedList.Count - 1; ++i)
        {
            // breed twice to keep population size the same
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

    private GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 pos = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);
        GameObject offspring = Instantiate(personPrefab, pos, Quaternion.identity);
        DNA dna1 = parent1.GetComponent<DNA>();
        DNA dna2 = parent2.GetComponent<DNA>();

        // randomly swap parent dna
        if (Random.Range(0, 1000) > 5)
        {
            offspring.GetComponent<DNA>().r = Random.Range(0, 10) < 5 ? dna1.r : dna2.r;
            offspring.GetComponent<DNA>().g = Random.Range(0, 10) < 5 ? dna1.g : dna2.g;
            offspring.GetComponent<DNA>().b = Random.Range(0, 10) < 5 ? dna1.b : dna2.b;
            offspring.GetComponent<DNA>().s = Random.Range(0, 10) < 5 ? dna1.s : dna2.s;
        }
        else
        {
            // apply mutation (very low probability)
            offspring.GetComponent<DNA>().r = Random.Range(0f, 1f);
            offspring.GetComponent<DNA>().g = Random.Range(0f, 1f);
            offspring.GetComponent<DNA>().b = Random.Range(0f, 1f);
            offspring.GetComponent<DNA>().s = Random.Range(0.1f, 0.3f);
        }

        return offspring;
    }

    private void OnGUI()
    {
        guiStyle.fontSize = 50;
        guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + generation, guiStyle);
        GUI.Label(new Rect(10, 65, 100, 20), "Trial Time: " + (int)elapsed, guiStyle);
    }
}
