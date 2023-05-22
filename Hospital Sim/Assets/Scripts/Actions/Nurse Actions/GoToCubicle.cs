using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToCubicle : GameAction
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
        GameWorld.Instance.GetWorld().ModifyState("treatingPatient", 1);
        inventory.RemoveItem(target);

        // recycle this cubicle
        GameWorld.Instance.GetQueue("cubicles").AddResource(target);
        GameWorld.Instance.GetWorld().ModifyState("freeCubicle", 1);
        return true;
    }
}
