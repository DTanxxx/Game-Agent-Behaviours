using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTreated : GameAction
{
    public override bool PrePerform()
    {
        // action fails if there is no free cubicle
        target = inventory.FindItemWithTag("Cubicle");
        if (target == null)
        {
            return false;
        }
        return true;
    }

    public override bool PostPerform()
    {
        GameWorld.Instance.GetWorld().ModifyState("treated", 1);

        beliefs.ModifyState("isCured", 1);
        // this action no longer needs a cubicle, free it
        inventory.RemoveItem(target);
        return true;
    }
}
