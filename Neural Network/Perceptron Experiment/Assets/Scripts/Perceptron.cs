using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainingSet
{
    public double[] input;
    public double output;
}

public class Perceptron : MonoBehaviour
{
    public TrainingSet[] trainingSets;
    public SimpleGrapher grapher;

    private double[] weights = { 0, 0 };
    private double bias = 0;
    private double totalError = 0;

    private void Start()
    {
        DrawAllPoints();
        Train(200);
        grapher.DrawRay((float)(-(bias / weights[1]) / (bias / weights[0])), (float)(-bias / weights[1]), Color.red);
        
        /*Debug.Log("Test 0 0: " + CalculateOutput(0, 0));
        Debug.Log("Test 0 1: " + CalculateOutput(0, 1));
        Debug.Log("Test 1 0: " + CalculateOutput(1, 0));
        Debug.Log("Test 1 1: " + CalculateOutput(1, 1));*/
        
        if (CalculateOutput(0.3, 0.9) == 0)
        {
            grapher.DrawPoint(0.3f, 0.9f, Color.red);
        }
        else
        {
            grapher.DrawPoint(0.3f, 0.9f, Color.yellow);
        }

        if (CalculateOutput(0.8, 0.1) == 0)
        {
            grapher.DrawPoint(0.8f, 0.1f, Color.red);
        }
        else
        {
            grapher.DrawPoint(0.8f, 0.1f, Color.yellow);
        }
    }

    private void DrawAllPoints()
    {
        for (int t = 0; t < trainingSets.Length; ++t)
        {
            if (trainingSets[t].output == 0)
            {
                grapher.DrawPoint((float)trainingSets[t].input[0], (float)trainingSets[t].input[1], Color.magenta);
            }
            else
            {
                grapher.DrawPoint((float)trainingSets[t].input[0], (float)trainingSets[t].input[1], Color.green);
            }
        }
    }

    private void InitializeWeights()
    {
        // set weights and bias to random values between -1 and 1
        for (int i = 0; i < weights.Length; ++i)
        {
            weights[i] = Random.Range(-1.0f, 1.0f);
        }
        bias = Random.Range(-1.0f, 1.0f);
    }

    private void Train(int epochs)
    {
        InitializeWeights();

        for (int e = 0; e < epochs; ++e)
        {
            totalError = 0;
            for (int t = 0; t < trainingSets.Length; ++t)
            {
                UpdateWeights(t);
                Debug.Log("W1: " + (weights[0]) + " W2: " + (weights[1]) + " B: " + bias);
            }
            Debug.Log("TOTAL ERROR: " + totalError);
        }
    }

    private void UpdateWeights(int index)
    {
        double error = trainingSets[index].output - CalculatePerceptronOutput(index);
        totalError += Mathf.Abs((float)error);
        for (int i = 0; i < weights.Length; ++i)
        {
            weights[i] = weights[i] + error * trainingSets[index].input[i];
        }
        bias += error;
    }

    private double DotProductBias(double[] weights, double[] inputs)
    {
        if (weights == null || inputs == null)
        {
            return -1;
        }

        if (weights.Length != inputs.Length)
        {
            return -1;
        }

        double d = 0;
        for (int x = 0; x < weights.Length; ++x)
        {
            d += weights[x] * inputs[x];
        }

        d += bias;
        return d;
    }

    private double CalculatePerceptronOutput(int i)
    {
        double dp = DotProductBias(weights, trainingSets[i].input);
        if (dp > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private double CalculateOutput(double input1, double input2)
    {
        double[] inputs = new double[] { input1, input2 };
        double dp = DotProductBias(weights, inputs);
        if (dp > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void Update()
    {
        
    }
}
