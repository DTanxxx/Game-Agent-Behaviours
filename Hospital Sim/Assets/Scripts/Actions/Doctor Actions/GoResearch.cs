using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoResearch : GameAction
{
    public override bool PrePerform()
    {
        // action fails if there is no free office
        target = GameWorld.Instance.GetQueue("offices").RemoveResource();
        if (target == null)
        {
            return false;
        }
        inventory.AddItem(target);
        GameWorld.Instance.GetWorld().ModifyState("freeOffice", -1);
        return true;
    }

    public override bool PostPerform()
    {
        inventory.RemoveItem(target);

        // recycle this office
        GameWorld.Instance.GetQueue("offices").AddResource(target);
        GameWorld.Instance.GetWorld().ModifyState("freeOffice", 1);
        return true;
    }
}
