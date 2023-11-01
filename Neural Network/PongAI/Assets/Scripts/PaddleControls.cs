using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleControls : MonoBehaviour 
{
	[SerializeField] private Brain parentBrain = null;

	private void Update () {
		if (parentBrain.human)
        {
			if (Input.GetKey("up"))
            {
				transform.Translate(0, 0.1f, 0);
			}	
			else if (Input.GetKey("down"))
            {
				transform.Translate(0, -0.1f, 0);
			}
		}
	}
}
