using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class TrainingSet
{
	public double[] input;
	public double output;
}

public class Perceptron : MonoBehaviour {

	public GameObject npc; 

	private List<TrainingSet> ts = new List<TrainingSet>();
	private double[] weights = { 0, 0 };
	private double bias = 0;
	private double totalError = 0;

	public void SendInput(double input1, double input2, double output)
    {
		// react
		double result = CalcOutput(input1, input2);
		Debug.Log(result);
		if (result == 0)
        {
			// duck for cover
			npc.GetComponent<Animator>().SetTrigger("Crouch");
			npc.GetComponent<Rigidbody>().isKinematic = false;
        }
		else
        {
			npc.GetComponent<Rigidbody>().isKinematic = true;
        }

		// learn from it for next time
		TrainingSet s = new TrainingSet();
		s.input = new double[2] { input1, input2 };
		s.output = output;
		ts.Add(s);
		Train();
    }

	private double DotProductBias(double[] weights, double[] values) 
	{
		if (weights == null || values == null)
			return -1;
	 
		if (weights.Length != values.Length)
			return -1;
	 
		double d = 0;
		for (int x = 0; x < weights.Length; x++)
		{
			d += weights[x] * values[x];
		}

		d += bias;
	 
		return d;
	}

	double CalcOutput(int i)
	{
		return ActivationFunction(DotProductBias(weights,ts[i].input));
	}

	double CalcOutput(double input1, double input2)
    {
		double[] inputs = new double[] { input1, input2 };
		return ActivationFunction(DotProductBias(weights, inputs));
    }

	double ActivationFunction(double dp)
	{
		if (dp > 0)
		{
			return (1);
		}

		return(0);
	}

	void InitialiseWeights()
	{
		for(int i = 0; i < weights.Length; i++)
		{
			weights[i] = Random.Range(-1.0f,1.0f);
		}
		bias = Random.Range(-1.0f,1.0f);
	}

	void UpdateWeights(int j)
	{
		double error = ts[j].output - CalcOutput(j);
		totalError += Mathf.Abs((float)error);
		for(int i = 0; i < weights.Length; i++)
		{			
			weights[i] = weights[i] + error*ts[j].input[i]; 
		}
		bias += error;
	}

	void Train()
	{
		for (int t = 0; t < ts.Count; t++)
		{
			UpdateWeights(t);
		}
	}

	private void LoadWeights()
    {
		string path = Application.dataPath + "/weights.txt";
		if (File.Exists(path))
        {
			var sr = File.OpenText(path);
			string line = sr.ReadLine();
			string[] w = line.Split(',');
			weights[0] = System.Convert.ToDouble(w[0]);
			weights[1] = System.Convert.ToDouble(w[1]);
			bias = System.Convert.ToDouble(w[2]);
			Debug.Log("loading");
        }
    }

	private void SaveWeights()
    {
		string path = Application.dataPath + "/weights.txt";
		var sr = File.CreateText(path);
		sr.WriteLine(weights[0] + "," + weights[1] + "," + bias);
		sr.Close();
    }

	void Start () {
		InitialiseWeights();
	}
	
	void Update () {
		if (Input.GetKeyDown("space"))
        {
			InitialiseWeights();
			ts.Clear();
        }
		else if (Input.GetKeyDown("s"))
        {
			SaveWeights();
        }
		else if (Input.GetKeyDown("l"))
        {
			LoadWeights();
        }
	}
}