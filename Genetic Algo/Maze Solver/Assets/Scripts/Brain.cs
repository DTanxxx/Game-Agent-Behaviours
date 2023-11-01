using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public DNA dna;
    public GameObject eyes;
    public float distanceTravelled = 0f;

    private int DNALength = 2;
    private bool seeWall = true;
    private Vector3 startPosition;
    private bool alive = true;

    public void Init()
    {
        // initialize DNA
        // 0 forward
        // 1 angle turn
        dna = new DNA(DNALength, 360);  // 360 possible values for the angle that the bot rotates by when hitting a wall
        startPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "dead")
        {
            distanceTravelled = 0f;
            alive = false;
        }
    }

    private void Update()
    {
        if (!alive)
        {
            return;
        }

        seeWall = false;
        RaycastHit hit;
        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 0.5f, Color.red);
        if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.forward, out hit, 0.5f))
        {
            if (hit.collider.gameObject.tag == "wall")
            {
                seeWall = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!alive)
        {
            return;
        }

        // read DNA
        float turn = 0f;
        float move = dna.GetGene(0);

        if (seeWall)
        {
            turn = dna.GetGene(1);
        }

        transform.Translate(0, 0, move * 0.001f);  // scale movement speed down since we have set the max value for each gene to be 360
        transform.Rotate(0, turn, 0);
        distanceTravelled = Vector3.Distance(startPosition, transform.position);
    }
}
