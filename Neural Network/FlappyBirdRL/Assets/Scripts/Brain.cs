using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// class to store past states for actions and associated rewards
public class Replay
{
    public List<double> states;
    public double reward;

	public Replay(double topDist, double botDist, double r)
	{
		states = new List<double>();
		states.Add(topDist);
		states.Add(botDist);
		reward = r;
	}
}

public class Brain : MonoBehaviour 
{
	[SerializeField] private GameObject topBeam;
	[SerializeField] private  GameObject bottomBeam;
	
	private ANN neuralNetwork;

	private float reward = 0.0f;
	private List<Replay> replayMemory = new List<Replay>();
	private int mCapacity = 10000;

	private float discount = 0.99f;
	private float exploreRate = 100.0f;
	private float maxExploreRate = 100.0f;
	private float minExploreRate = 0.01f;
	private float exploreDecay = 0.0001f;

	private int failCount = 0;
	private float moveForce = 0.5f;

	private float timer = 0;
	private float maxBalanceTime = 0;

	private bool crashed = false;
	private Vector3 startPos;
	private Rigidbody2D rb;

	private void Start() 
	{
		neuralNetwork = new ANN(2,2,1,6,0.2f);
		startPos = transform.position;
		Time.timeScale = 5.0f;
		rb = GetComponent<Rigidbody2D>();
	}

	private void Update()
	{
		if(Input.GetKeyDown("space"))
			ResetBird();
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		crashed = true;
	}

	private void OnCollisionExit2D(Collision2D col)
	{
		crashed = false;
	}

	private void FixedUpdate() 
	{
		timer += Time.deltaTime;
		List<double> states = new List<double>();
		List<double> qs = new List<double>();
			
		// 2 inputs
		states.Add(Vector3.Distance(transform.position, topBeam.transform.position));
		states.Add(Vector3.Distance(transform.position, bottomBeam.transform.position));
		
		qs = SoftMax(neuralNetwork.CalcOutput(states));
		double maxQ = qs.Max();
		int maxQIndex = qs.ToList().IndexOf(maxQ);
		exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

		if(Random.Range(0,100) < exploreRate)
        {
			maxQIndex = Random.Range(0, 2);
		}
			
		// 2 outputs
		if(maxQIndex == 0)
        {
			rb.AddForce(Vector3.up * moveForce * (float)qs[maxQIndex]);
		}
		else if (maxQIndex == 1)
        {
			rb.AddForce(Vector3.up * -moveForce * (float)qs[maxQIndex]);
		}
		
		if(crashed)
        {
			reward = -1.0f;
		}	
		else
        {
			reward = 0.1f;
		}

		Replay lastMemory = new Replay(Vector3.Distance(transform.position, topBeam.transform.position),
								Vector3.Distance(transform.position, bottomBeam.transform.position),
								reward);

		if(replayMemory.Count > mCapacity)
        {
			replayMemory.RemoveAt(0);
		}

		replayMemory.Add(lastMemory);

		if (crashed) 
		{
			for(int i = replayMemory.Count - 1; i >= 0; i--)
			{
				List<double> toutputsOld = new List<double>();
				List<double> toutputsNew = new List<double>();
				toutputsOld = SoftMax(neuralNetwork.CalcOutput(replayMemory[i].states));	

				double maxQOld = toutputsOld.Max();
				int action = toutputsOld.ToList().IndexOf(maxQOld);

			    double feedback;
				if(i == replayMemory.Count-1 || replayMemory[i].reward == -1)
                {
					feedback = replayMemory[i].reward;
				}
				else
				{
					toutputsNew = SoftMax(neuralNetwork.CalcOutput(replayMemory[i+1].states));
					maxQ = toutputsNew.Max();
					feedback = (replayMemory[i].reward + discount * maxQ);
				} 

				toutputsOld[action] = feedback;
				neuralNetwork.Train(replayMemory[i].states,toutputsOld);
			}
		
			if (timer > maxBalanceTime)
			{
			 	maxBalanceTime = timer;
			} 

			timer = 0;

			crashed = false;
			ResetBird();
			replayMemory.Clear();
			failCount++;
		}	
	}

	private void ResetBird()
	{
		transform.position = startPos;
		GetComponent<Rigidbody2D>().velocity = new Vector3(0,0,0);
	}

	private List<double> SoftMax(List<double> oSums) 
    {
		double max = oSums.Max();

		float scale = 0.0f;
		for (int i = 0; i < oSums.Count; ++i)
		{
			scale += Mathf.Exp((float)(oSums[i] - max));
		}
        
		List<double> result = new List<double>();
		for (int i = 0; i < oSums.Count; ++i)
        {
			result.Add(Mathf.Exp((float)(oSums[i] - max)) / scale);
		}
        
		return result; 
    }
}
