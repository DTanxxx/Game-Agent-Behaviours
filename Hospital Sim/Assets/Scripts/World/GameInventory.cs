using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInventory
{
    public List<GameObject> items = new List<GameObject>();

    public void AddItem(GameObject i)
    {
        items.Add(i);
    }

    public GameObject FindItemWithTag(string tag)
    {
        foreach (GameObject i in items)
        {
            if (i == null)
            {
                break;
            }

            if (i.tag == tag)
            {
                return i;
            }
        }
        return null;
    }

    public void RemoveItem(GameObject i)
    {
        int indexToRemove = -1;
        foreach (GameObject obj in items)
        {
            indexToRemove += 1;
            if (obj == i)
            {
                break;
            }
        }
        if (indexToRemove > -1)
        {
            items.RemoveAt(indexToRemove);
        }
    }
}
