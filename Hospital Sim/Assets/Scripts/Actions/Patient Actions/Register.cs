using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : GameAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        beliefs.ModifyState("atHospital", 0);
        return true;
    }
}
