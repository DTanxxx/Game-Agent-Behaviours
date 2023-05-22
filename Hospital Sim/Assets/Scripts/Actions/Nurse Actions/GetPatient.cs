using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPatient : GameAction
{
    private GameObject resource;

    public override bool PrePerform()
    {
        // set the target as one of the patients
        target = GameWorld.Instance.GetQueue("patients").RemoveResource();

        if (target == null)
        {
            // no patient available, fail this action so planner can replan
            return false;
        }

        // check if we have a free cubicle resource
        resource = GameWorld.Instance.GetQueue("cubicles").RemoveResource();
        if (resource != null)
        {
            inventory.AddItem(resource);
        }
        else
        {
            // no free cubicle, fail this action
            // "release" this patient instance since the nurse cannot bring them to the cubicle
            GameWorld.Instance.GetQueue("patients").AddResource(target);
            target = null;
            return false;
        }

        // use a free cubicle
        GameWorld.Instance.GetWorld().ModifyState("freeCubicle", -1);
        return true;
    }

    public override bool PostPerform()
    {
        // use a waiting patient
        GameWorld.Instance.GetWorld().ModifyState("waiting", -1);
        if (target)
        {
            // only give patient a cubicle to go to after this nurse has finished this action
            target.GetComponent<GameAgent>().inventory.AddItem(resource);
        }
        return true;
    }
}
