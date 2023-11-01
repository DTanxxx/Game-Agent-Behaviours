using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    private ANN neuralNetwork;
    private double sumSquareError = 0;  // indicates how closely our predicted data fits the actual data

    private void Start()
    {
        neuralNetwork = new ANN(2, 1, 1, 2, 0.8);

        List<double> result;

        // run 30000 epochs of training sets for XOR operation
        for (int i = 0; i < 30000; ++i)
        {
            sumSquareError = 0;
            result = Train(1, 1, 0);
            sumSquareError += Mathf.Pow((float)result[0] - 0, 2);
            result = Train(1, 0, 1);
            sumSquareError += Mathf.Pow((float)result[0] - 1, 2);
            result = Train(0, 1, 1);
            sumSquareError += Mathf.Pow((float)result[0] - 1, 2);
            result = Train(0, 0, 0);
            sumSquareError += Mathf.Pow((float)result[0] - 0, 2);
        }
        Debug.Log("SSE: " + sumSquareError);

        result = Train(1, 1, 0);
        Debug.Log(" 1 1 " + result[0]);
        result = Train(1, 0, 1);
        Debug.Log(" 1 0 " + result[0]);
        result = Train(0, 1, 1);
        Debug.Log(" 0 1 " + result[0]);
        result = Train(0, 0, 0);
        Debug.Log(" 0 0 " + result[0]);
    }

    private List<double> Train(double input1, double input2, double output)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();
        inputs.Add(input1);
        inputs.Add(input2);
        outputs.Add(output);
        return neuralNetwork.Run(inputs, outputs);
    }
}
