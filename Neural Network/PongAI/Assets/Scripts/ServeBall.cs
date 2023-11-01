using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServeBall : MonoBehaviour 
{
	[SerializeField] private GameObject ball;
	[SerializeField] private bool backWall = false;
	[SerializeField] private Brain b;

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.CompareTag("ball") && backWall)
		{
			b.numBallsMissed += 1;
			ball.GetComponent<MoveBall>().ResetBall();
		}
	}
}
