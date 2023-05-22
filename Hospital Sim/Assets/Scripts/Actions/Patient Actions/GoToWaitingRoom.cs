using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToWaitingRoom : GameAction
{
    public override bool PrePerform()
    {
        return true;
    }

    public override bool PostPerform()
    {
        // add a "waiting" state to the world states so nurses can start their actions
        GameWorld.Instance.GetWorld().ModifyState("waiting", 1);

        // add this patient game object to the world so nurses can grab reference to them
        GameWorld.Instance.GetQueue("patients").AddResource(gameObject);

        beliefs.ModifyState("atHospital", 1);
        return true;
    }
}
