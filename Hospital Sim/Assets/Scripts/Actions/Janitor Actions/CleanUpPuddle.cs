using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanUpPuddle : GameAction
{
    public override bool PrePerform()
    {
        target = GameWorld.Instance.GetQueue("puddles").RemoveResource();

        if (target == null)
        {
            return false;
        }
        inventory.AddItem(target);
        GameWorld.Instance.GetWorld().ModifyState("freePuddle", -1);
        return true;
    }

    public override bool PostPerform()
    {
        inventory.RemoveItem(target);
        Destroy(target, duration);
        return true;
    }
}
