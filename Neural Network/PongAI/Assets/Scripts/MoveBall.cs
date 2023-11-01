using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBall : MonoBehaviour 
{
	[SerializeField] private AudioSource blip;
	[SerializeField] private AudioSource blop;

	private Vector3 ballStartPosition;
	private Rigidbody2D rb;
	private float force = 400.0f;

    private void Start()
    {
		rb = GetComponent<Rigidbody2D>();
		ballStartPosition = transform.position;
		ResetBall();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("backwall"))
        {
            blop.Play();
        }
        else
        {
            blip.Play();
        }
    }

    public void ResetBall()
    {
        transform.position = ballStartPosition;
        rb.velocity = Vector3.zero;
        Vector3 dir = new Vector3(Random.Range(100, 300), Random.Range(-100, 100), 0).normalized;
        rb.AddForce(dir * force);
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ResetBall();
        }
    }
}
