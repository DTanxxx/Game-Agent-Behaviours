using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    private int DNALength = 2;  // 2 genes in a DNA, one is used to determine what the agent should do when seeing ground, and one to determine what the agent should do when not seeing ground
    private bool alive = true;
    private bool seeGround = true;
    private GameObject ethanInstance;

    public float timeAlive;
    public float timeWalking;
    public DNA dna;
    public GameObject eyes;
    public GameObject ethanPrefab;

    private void OnDestroy()
    {
        Destroy(ethanInstance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "dead")
        {
            alive = false;
            timeAlive = 0f;
            timeWalking = 0f;
        }
    }

    public void Init()
    {
        // initialize DNA
        // 0: forward
        // 1: left
        // 2: right
        dna = new DNA(DNALength, 3);  // create a DNA instance of length 2 and gene max value (exclusive) of 3
        timeAlive = 0;
        alive = true;
        ethanInstance = Instantiate(ethanPrefab, transform.position, transform.rotation);
        ethanInstance.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().target = transform;
    }

    private void Update()
    {
        if (!alive)
        {
            return;
        }

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10, Color.red, 10);
        seeGround = false;
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit))
        {
            if (hit.collider.gameObject.tag == "platform")
            {
                seeGround = true;
            }
        }
        timeAlive = PopulationManager.elapsed;

        // read DNA
        float turn = 0;  // rotation
        float move = 0;  // velocity
        if (seeGround)
        {
            // make v relative to character and always move forward
            if (dna.GetGene(0) == 0)
            {
                move = 1;
                timeWalking += 1f;
            }
            else if (dna.GetGene(0) == 1)
            {
                turn = -90;
            }
            else if (dna.GetGene(0) == 2)
            {
                turn = 90;
            }
        }
        else
        {
            if (dna.GetGene(1) == 0)
            {
                move = 1;
                timeWalking += 1f;
            }
            else if (dna.GetGene(1) == 1)
            {
                turn = -90;
            }
            else if (dna.GetGene(1) == 2)
            {
                turn = 90;
            }
        }

        transform.Translate(0, 0, move * 0.1f);  // move forward
        transform.Rotate(0, turn, 0);  // rotate around y axis
    }
}
