using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Blackboard : MonoBehaviour
{
    public TextMeshProUGUI clock;
    public float timeOfDay;
    public Stack<GameObject> patrons = new Stack<GameObject>();
    public int openTime = 6;
    public int closeTime = 20;

    private WaitForSeconds clockWait;

    // set up Blackboard singleton
    private static Blackboard instance;
    public static Blackboard Instance
    {
        get
        {
            if (!instance)
            {
                Blackboard[] blackboards = FindObjectsOfType<Blackboard>();
                if (blackboards != null)
                {
                    if (blackboards.Length == 1)
                    {
                        instance = blackboards[0];
                        return instance;
                    }
                }
                GameObject obj = new GameObject("Blackboard", typeof(Blackboard));
                instance = obj.GetComponent<Blackboard>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
        set
        {
            instance = value;
        }
    }

    private void Start()
    {
        clockWait = new WaitForSeconds(3);
        StartCoroutine(UpdateClock());
    }

    private IEnumerator UpdateClock()
    {
        while (true)
        {
            timeOfDay += 1;
            if (timeOfDay > 23)
            {
                timeOfDay = 0;
            }
            clock.text = timeOfDay + ":00";
            if (timeOfDay == closeTime)
            {
                patrons.Clear();
            }
            yield return clockWait;
        }
    }

    public bool RegisterPatron(GameObject p)
    {
        patrons.Push(p);
        return true;
    }
}
