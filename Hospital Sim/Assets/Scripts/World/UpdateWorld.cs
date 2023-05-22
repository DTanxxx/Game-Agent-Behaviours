using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateWorld : MonoBehaviour
{
    public Text states;

    private void LateUpdate()
    {
        Dictionary<string, int> worldStates = GameWorld.Instance.GetWorld().GetStates();
        states.text = "";
        foreach (KeyValuePair<string, int> state in worldStates)
        {
            states.text += state.Key + ", " + state.Value + "\n";
        }
    }
}
