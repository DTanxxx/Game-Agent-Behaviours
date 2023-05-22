using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Janitor : GameAgent
{
    protected override void Start()
    {
        base.Start();
        SubGoal s1 = new SubGoal("cleanPuddle", 1, false);
        goals.Add(s1, 1);
    }
}
