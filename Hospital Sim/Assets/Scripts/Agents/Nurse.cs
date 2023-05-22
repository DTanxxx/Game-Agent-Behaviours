using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nurse : GameAgent
{
    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("treatPatient", 1, false);  // this goal should not be removable so this nurse can carry out this goal on many patients
        goals.Add(s1, 3);

        SubGoal s2 = new SubGoal("rested", 1, false);
        goals.Add(s2, 1);  // this goal has lower priority than treatPatient, this means as long as there is patient to treat, this goal will not be considered by planner

        SubGoal s3 = new SubGoal("relief", 1, false);
        goals.Add(s3, 5); 

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
