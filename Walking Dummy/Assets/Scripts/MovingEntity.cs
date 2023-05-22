using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : MonoBehaviour
{
    protected Vector3 headingVector = Vector3.zero;
    protected Vector3 sideVector = Vector3.zero;  // perpendicular to the heading vector
    protected Vector3 velocity = Vector3.zero;
    [SerializeField] protected float mass;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float maxForce;
    [SerializeField] protected float maxTurnRate;

    private NavGraph navGraph = null;
    private SteeringBehaviours steeringBehaviours = null;

    private bool collided = false;

    private void Start()
    {
        steeringBehaviours = GetComponent<SteeringBehaviours>();    
    }

    private void Update()
    {
        if (navGraph == null)
        {
            navGraph = FindObjectOfType<GraphGenerator>().GetGraph();
        }

        if (collided)
        {
            return;
        }

        // calculate the combined force from each steering behaviour
        Vector3 steeringForce = steeringBehaviours.CalculateTotalForce();
        Vector3 acceleration = new Vector3(steeringForce.x / mass, steeringForce.y / mass, steeringForce.z / mass);
        velocity += acceleration * Time.deltaTime;
        if (steeringForce == Vector3.zero)
        {
            velocity = Vector3.zero;
        }

        if (Mathf.Min(velocity.magnitude, maxSpeed) == maxSpeed)
        {
            velocity = velocity.normalized;
            velocity.Scale(Vector3.one * maxSpeed);
        }

        // Check if the next update on transform.position would be touching an obstacle
        var move = velocity * Time.deltaTime;
        move.Scale(ContactWithObstacle(move));
        transform.position += move;

        if (velocity.magnitude > 0.000001)
        {
            headingVector = headingVector.normalized;
            headingVector = Vector3.Lerp(headingVector, velocity.normalized, maxTurnRate);
            sideVector = new Vector3(headingVector.z, 0, -headingVector.x);
            // update player's rotation too
            transform.rotation = Quaternion.LookRotation(headingVector);
        }
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    public Vector3 GetVelocity()
    {
        return velocity;
    }

    public Vector3 GetHeadingVector()
    {
        return headingVector;
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
}
