using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public float distanceTravelled = 0;  // used to determine fitness
    public int crash = 0;  // used to determine fitness
    public DNA dna;
    public GameObject eyes;
    public float timeAlive = 0;

    private int DNALength = 5;
    private bool seeDownWall = false;
    private bool seeUpWall = false;
    private bool seeBottom = false;
    private bool seeTop = false;
    private Vector3 startPosition;
    private bool alive = true;  
    private Rigidbody2D rb;    
    
	public void Init()
	{
		// initialise DNA
        // 0 forward
        // 1 upwall
        // 2 downwall
        // 3 normal upward
        dna = new DNA(DNALength,200);  // force value is from 0 to 200
        this.transform.Translate(Random.Range(-1.5f,1.5f),Random.Range(-1.5f,1.5f),0);
        startPosition = this.transform.position;
        rb = this.GetComponent<Rigidbody2D>();
	}

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "dead")
        {
            alive = false;
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.tag == "top" ||
            col.gameObject.tag == "bottom" ||
            col.gameObject.tag == "upwall" ||
            col.gameObject.tag == "downwall")
        {
            crash++;
        }
    }

    private void Update()
    {
        if (!alive)
        {
            return;
        }

        seeUpWall = false;
        seeDownWall = false;
        seeTop = false;
        seeBottom = false;

        // sense forward
        RaycastHit2D hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.forward, 1.0f);

        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 1.0f, Color.red);
        Debug.DrawRay(eyes.transform.position, eyes.transform.up* 1.0f, Color.red);
        Debug.DrawRay(eyes.transform.position, -eyes.transform.up* 1.0f, Color.red);

        if (hit.collider != null)
        {
            if(hit.collider.gameObject.tag == "upwall")
            {
                seeUpWall = true;
            }
            else if(hit.collider.gameObject.tag == "downwall")
            {
                seeDownWall = true;
            }
        }

        // sense upward
		hit = Physics2D.Raycast(eyes.transform.position, eyes.transform.up, 1.0f);
		if (hit.collider != null)
        {
            if(hit.collider.gameObject.tag == "top")
            {
                seeTop = true;
            }
        }

        // sense downward
        hit = Physics2D.Raycast(eyes.transform.position, -eyes.transform.up, 1.0f);
		if (hit.collider != null)
        {    
            if(hit.collider.gameObject.tag == "bottom")
            {
                seeBottom = true;
            }
        }
        timeAlive = PopulationManager.elapsed;
    }


    private void FixedUpdate()
    {
        if (!alive) 
        { 
            return; 
        }
        
        // read DNA
        float upForce = 0;
        float forwardForce = 1.0f;

        if(seeUpWall)
        { 
            upForce = dna.GetGene(0);
        }
        else if(seeDownWall)
        {
        	upForce = dna.GetGene(1);
        }
        else if(seeTop)
        {
        	upForce = dna.GetGene(2);
        }        
        else if(seeBottom)
        {
        	upForce = dna.GetGene(3);
        }
        else
        {
        	upForce = dna.GetGene(4);  // default up force
        }

        rb.AddForce(this.transform.right * forwardForce);
        rb.AddForce(this.transform.up * upForce * 0.1f);
        distanceTravelled = Vector3.Distance(startPosition,this.transform.position);
    }
}

