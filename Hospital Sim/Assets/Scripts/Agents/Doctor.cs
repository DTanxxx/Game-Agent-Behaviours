using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doctor : GameAgent
{
    protected override void Start()
    {
        base.Start();

        SubGoal s1 = new SubGoal("rested", 1, false);
        goals.Add(s1, 3);

        SubGoal s2 = new SubGoal("research", 1, false);
        goals.Add(s2, 1);

        SubGoal s3 = new SubGoal("relief", 1, false);
        goals.Add(s3, 2);

        Invoke("GetTired", Random.Range(10, 20));
        Invoke("NeedRelief", Random.Range(10, 20));
    }

    private void GetTired()
    {
        beliefs.ModifyState("exhausted", 0);
        Invoke("GetTired", Random.Range(10, 20));
    }

    private void NeedRelief()
    {
        beliefs.ModifyState("busting", 0);
        Invoke("NeedRelief", Random.Range(10, 20));
    }
}
