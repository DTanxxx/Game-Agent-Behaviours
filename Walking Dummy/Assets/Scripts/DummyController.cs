using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && 
            GetComponent<SteeringBehaviours>().GetBehaviourOn() != BehaviorOn.Pursuit &&
            GetComponent<SteeringBehaviours>().GetBehaviourOn() != BehaviorOn.Evade)
        {
            // Get the mouse's position
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                if (objectHit.gameObject.tag == "Ground")
                {
                    GetComponent<SteeringBehaviours>().SetTargetPosition(hit.point + 
                        new Vector3(0, GetComponent<CapsuleCollider>().height / 2, 0));
                }
            }
        }
        else if (GetComponent<SteeringBehaviours>().GetBehaviourOn() == BehaviorOn.Pursuit
            || GetComponent<SteeringBehaviours>().GetBehaviourOn() == BehaviorOn.Evade)
        {
            GetComponent<SteeringBehaviours>().SetTargetPosition(transform.position);
        }
    }
}
