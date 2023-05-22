using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaderController : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float deceleration = 0.5f;

    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(horizontal, 0.0f, vertical) * speed * Time.deltaTime;
        move.Scale(ContactWithObstacle(move));
        transform.position += move;
        velocity = move;
    }

    private Vector3 ContactWithObstacle(Vector3 deltaPos)
    {
        Collider[] collidersHorizontal = 
            Physics.OverlapSphere(transform.position + new Vector3(deltaPos.x, 0, 0), GetComponent<CapsuleCollider>().radius);
        Collider[] collidersFrontBack = 
            Physics.OverlapSphere(transform.position + new Vector3(0, 0, deltaPos.z), GetComponent<CapsuleCollider>().radius);

        Vector3 multiplier = Vector3.one;

        foreach (var collider in collidersHorizontal)
        {
            if (collider.gameObject.tag == "Obstacle")
            {
                multiplier.x = 0;
                break;
            }
        }
        foreach (var collider in collidersFrontBack)
        {
            if (collider.gameObject.tag == "Obstacle")
            {
                multiplier.z = 0;
                break;
            }
        }
        return multiplier;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }
}
