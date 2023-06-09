using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    private List<int> genes = new List<int>();  // a list of values for controlling movement
    private int dnaLength = 0;
    private int maxValues = 0;

    public DNA(int l, int v)
    {
        dnaLength = l;
        maxValues = v;
        SetRandom();
    }

    public void SetRandom()
    {
        genes.Clear();
        for (int i = 0; i < dnaLength; ++i)
        {
            genes.Add(Random.Range(0, maxValues));
        }
    }

    public void SetInt(int pos, int value)
    {
        genes[pos] = value;
    }

    public void Combine(DNA d1, DNA d2)
    {
        for (int i = 0; i < dnaLength; ++i)
        {
            // first half of this DNA's genes will come from d1; second half will come from d2
            if (i < dnaLength / 2.0)
            {
                int c = d1.genes[i];
                genes[i] = c;
            }
            else
            {
                int c = d2.genes[i];
                genes[i] = c;
            }
        }
    }

    public void Mutate()
    {
        // mutate a random value in genes list
        genes[Random.Range(0, dnaLength)] = Random.Range(0, maxValues);
    }

    public int GetGene(int pos)
    {
        return genes[pos];
    }
}
