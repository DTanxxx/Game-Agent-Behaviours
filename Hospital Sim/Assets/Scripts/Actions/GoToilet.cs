using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToilet : GameAction
{
    public override bool PrePerform()
    {
        // action fails if there is no free office
        target = GameWorld.Instance.GetQueue("toilets").RemoveResource();
        if (target == null)
        {
            return false;
        }
        inventory.AddItem(target);
        GameWorld.Instance.GetWorld().ModifyState("freeToilet", -1);
        return true;
    }

    public override bool PostPerform()
    {
        inventory.RemoveItem(target);

        // recycle this toilet
        GameWorld.Instance.GetQueue("toilets").AddResource(target);
        GameWorld.Instance.GetWorld().ModifyState("freeToilet", 1);
        beliefs.RemoveState("busting");
        return true;
    }
}
