using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANN
{
    public int numInputs;
    public int numOutputs;
    public int numHiddenLayers;
    public int numNodesPerHiddenLayer;
    public double learningRate;

    private List<Layer> layers = new List<Layer>();

    public ANN(int nInputs, int nOutputs, int nHiddenLayers, int nNodesPerHidden, double lRate)
    {
        numInputs = nInputs;
        numOutputs = nOutputs;
        numHiddenLayers = nHiddenLayers;
        numNodesPerHiddenLayer = nNodesPerHidden;
        learningRate = lRate;

        if (numHiddenLayers > 0)
        {
            // add first hidden layer
            layers.Add(new Layer(numNodesPerHiddenLayer, numInputs));

            for (int i = 0; i < numHiddenLayers - 1; ++i)
            {
                // add middle hidden layers
                layers.Add(new Layer(numNodesPerHiddenLayer, numNodesPerHiddenLayer));
            }

            // add output layer
            layers.Add(new Layer(numOutputs, numNodesPerHiddenLayer));
        }
        else
        {
            // no hidden layers, add output layer right away
            layers.Add(new Layer(numOutputs, numInputs));
        }
    }

    public List<double> Run(List<double> inputValues, List<double> desiredOutputs)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        if (inputValues.Count != numInputs)
        {
            Debug.Log("ERROR: Number of Inputs must be " + numInputs.ToString());
            return outputs;
        }

        inputs = new List<double>(inputValues);
        // iterate through each hidden layer
        for (int i = 0; i < numHiddenLayers + 1; ++i)
        {
            if (i > 0)
            {
                // not the first hidden layer, use the existing outputs as this layer's inputs
                inputs = new List<double>(outputs);
            }

            // clear the outputs
            outputs.Clear();

            // iterate through each neuron in this hidden layer
            for (int j = 0; j < layers[i].numNeurons; ++j)
            {
                double dotProduct = 0;
                layers[i].neurons[j].inputs.Clear();
                
                // iterate through each input in this neuron
                for (int k = 0; k < layers[i].neurons[j].numInputs; ++k)
                {
                    layers[i].neurons[j].inputs.Add(inputs[k]);
                    dotProduct += layers[i].neurons[j].weights[k] * inputs[k];
                }

                dotProduct -= layers[i].neurons[j].bias;

                if (i == numHiddenLayers)
                {
                    // use a different activation function for output layer
                    layers[i].neurons[j].output = ActivationFunctionO(dotProduct);
                }
                else
                {
                    // use normal sigmoid function for hidden layers
                    layers[i].neurons[j].output = ActivationFunction(dotProduct);
                }
                
                outputs.Add(layers[i].neurons[j].output);
            }
        }

        UpdateWeights(outputs, desiredOutputs);

        return outputs;
    }

    private void UpdateWeights(List<double> outputs, List<double> desiredOutputs)
    {
        double error;

        // loop backwards from output to input layer - backpropagation
        for (int i = numHiddenLayers; i >= 0; --i)
        {
            for (int j = 0; j < layers[i].numNeurons; ++j)
            {
                // first step: calculate error for each neuron
                if (i == numHiddenLayers)
                {
                    // if we are in the output layer, determine what the error is
                    error = desiredOutputs[j] - outputs[j];
                    layers[i].neurons[j].errorGradient = outputs[j] * (1 - outputs[j]) * error;
                    // errorGradient calculated with Delta Rule
                }
                else
                {
                    layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * (1 - layers[i].neurons[j].output);
                    
                    // calculate the error gradients from the layer after this layer
                    double errorGradSum = 0;
                    for (int p = 0; p < layers[i + 1].numNeurons; ++p)
                    {
                        errorGradSum += layers[i + 1].neurons[p].errorGradient * layers[i + 1].neurons[p].weights[j];
                    }

                    // multiply the error gradient sum to this neuron's error gradient
                    layers[i].neurons[j].errorGradient *= errorGradSum;
                }

                // second step: update weight for each neuron
                for (int k = 0; k < layers[i].neurons[j].numInputs; ++k)
                {
                    if (i == numHiddenLayers)
                    {
                        error = desiredOutputs[j] - outputs[j];
                        layers[i].neurons[j].weights[k] += learningRate * layers[i].neurons[j].inputs[k] * error;
                    }
                    else
                    {
                        layers[i].neurons[j].weights[k] += learningRate * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
                    }
                }
                layers[i].neurons[j].bias += learningRate * -1 * layers[i].neurons[j].errorGradient;
            }
        }
    }

    // activation functions

    // activation function for hidden layers
    private double ActivationFunction(double value)
    {
        return ReLu(value);
    }

    // activation function for output layer
    private double ActivationFunctionO(double value)
    {
        return Sigmoid(value);
    }

    // binary step
    private double Step(double value)
    {
        if (value < 0)
        {
            return 0;
        }
        else
        {
            return 1;
        }
    }

    // logistic softstep
    private double Sigmoid(double value)
    {
        double k = (double)System.Math.Exp(value);
        return k / (1.0f + k);
    }

    // tanh's output includes negative values
    private double TanH(double value)
    {
        return 2 * (Sigmoid(2 * value)) - 1;
    }

    private double ReLu(double value)
    {
        if (value > 0)
        {
            return value;
        }
        else
        {
            return 0;
        }
    }

    private double LeakyReLu(double value)
    {
        if (value < 0)
        {
            return 0.01 * value;
        }
        else
        {
            return value;
        }
    }

    private double Sinusoid(double value)
    {
        return Mathf.Sin((float)value);
    }

    private double ArcTan(double value)
    {
        return Mathf.Atan((float)value);
    }

    private double SoftSign(double value)
    {
        return value / (1 + Mathf.Abs((float)value));
    }
}
