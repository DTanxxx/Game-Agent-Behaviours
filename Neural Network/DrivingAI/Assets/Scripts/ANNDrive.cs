using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ANNDrive : MonoBehaviour
{
    [SerializeField] private float visibleDistance = 200;
    [SerializeField] private int epochs = 1000;
    [SerializeField] private float speed = 50.0f;
    [SerializeField] private float rotationSpeed = 100.0f;
    [SerializeField] private float translation;
    [SerializeField] private float rotation;
    [SerializeField] private bool loadFromFile = false;

    private ANN neuralNetwork;
    private bool trainingDone = false;
    private float trainingProgress = 0f;
    private double sse = 0;
    private double lastSSE = 1;

    private void Start()
    {
        neuralNetwork = new ANN(5, 2, 1, 10, 0.5);
        if (loadFromFile)
        {
            LoadWeightsFromFile();
            trainingDone = true;
        }
        else
        {
            StartCoroutine(LoadTrainingSet());
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + lastSSE);
        GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + neuralNetwork.learningRate);
        GUI.Label(new Rect(25, 55, 250, 30), "Trained: " + trainingProgress);
    }

    private IEnumerator LoadTrainingSet()
    {
        string path = Application.dataPath + "/trainingData.txt";
        string line;
        if (File.Exists(path))
        {
            int lineCount = File.ReadAllLines(path).Length;
            StreamReader trainingDataReader = File.OpenText(path);

            List<double> calcOutputs = new List<double>();
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            for (int i = 0; i < epochs; ++i)
            {
                // set file pointer to beginning of file
                sse = 0;
                trainingDataReader.BaseStream.Position = 0;
                string currentWeights = neuralNetwork.PrintWeights();

                while ((line = trainingDataReader.ReadLine()) != null)
                {
                    string[] data = line.Split(',');

                    // if nothing to be learned ignore this line
                    float thisError = 0;
                    if (!(System.Convert.ToDouble(data[5]) == 0 && System.Convert.ToDouble(data[6]) == 0))
                    {
                        inputs.Clear();
                        outputs.Clear();

                        inputs.Add(System.Convert.ToDouble(data[0]));
                        inputs.Add(System.Convert.ToDouble(data[1]));
                        inputs.Add(System.Convert.ToDouble(data[2]));
                        inputs.Add(System.Convert.ToDouble(data[3]));
                        inputs.Add(System.Convert.ToDouble(data[4]));

                        // normalize the desired output values to between [0,1]
                        double output1 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[5]));
                        outputs.Add(output1);
                        double output2 = Map(0, 1, -1, 1, System.Convert.ToSingle(data[6]));
                        outputs.Add(output2);

                        calcOutputs = neuralNetwork.Train(inputs, outputs);

                        // error is the average of mean squared differences
                        thisError = ((Mathf.Pow((float)(outputs[0] - calcOutputs[0]), 2) +
                            Mathf.Pow((float)(outputs[1] - calcOutputs[1]), 2))) / 2.0f;
                    }

                    sse += thisError;
                }

                // calculate percentage of progress in training
                trainingProgress = (float)i / (float)epochs;

                // sse is the average of total sse for all training data lines
                sse /= lineCount;
                
                // if sse isn't better, then reload previous set of weights and decrease alpha so 
                // we can head towards the bottom of local optimum
                if (lastSSE < sse)
                {
                    neuralNetwork.LoadWeights(currentWeights);
                    neuralNetwork.learningRate = Mathf.Clamp((float)neuralNetwork.learningRate - 0.001f, 0.01f, 0.9f);
                }
                else
                {
                    // increase alpha to train faster
                    neuralNetwork.learningRate = Mathf.Clamp((float)neuralNetwork.learningRate + 0.001f, 0.01f, 0.9f);
                    lastSSE = sse;
                }

                yield return null;
            }
        }

        trainingDone = true;
        SaveWeightsToFile();
    }

    private void SaveWeightsToFile()
    {
        string path = Application.dataPath + "/weights.txt";
        StreamWriter writer = File.CreateText(path);
        writer.WriteLine(neuralNetwork.PrintWeights());
        writer.Close();
    }

    private void LoadWeightsFromFile()
    {
        string path = Application.dataPath + "/weights.txt";
        StreamReader reader = File.OpenText(path);

        if (File.Exists(path))
        {
            string line = reader.ReadLine();
            neuralNetwork.LoadWeights(line);
        }
    }

    private float Map(float newFrom, float newTo, float origFrom, float origTo, float value)
    {
        if (value <= origFrom)
        {
            return newFrom;
        }
        else if (value >= origTo)
        {
            return newTo;
        }
        else
        {
            return (newTo - newFrom) * ((value - origFrom) / (origTo - origFrom)) + newFrom;
        }
    }

    private float Round(float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
    }

    private void Update()
    {
        if (!trainingDone)
        {
            return;
        }

        List<double> calcOutputs = new List<double>();
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        // raycasts
        RaycastHit hit;

        float forwardDistance = 0;
        float rightDistance = 0;
        float leftDistance = 0;
        float rightDiagDistance = 0;
        float leftDiagDistance = 0;

        // forward
        if (Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
        {
            forwardDistance = 1 - Round(hit.distance / visibleDistance);
        }

        // right
        if (Physics.Raycast(transform.position, transform.right, out hit, visibleDistance))
        {
            rightDistance = 1 - Round(hit.distance / visibleDistance);
        }

        // left
        if (Physics.Raycast(transform.position, -transform.right, out hit, visibleDistance))
        {
            leftDistance = 1 - Round(hit.distance / visibleDistance);
        }

        // right diagonal
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * transform.right, out hit, visibleDistance))
        {
            rightDiagDistance = 1 - Round(hit.distance / visibleDistance);
        }

        // left diagonal
        if (Physics.Raycast(transform.position, Quaternion.AngleAxis(45, Vector3.up) * -transform.right, out hit, visibleDistance))
        {
            leftDiagDistance = 1 - Round(hit.distance / visibleDistance);
        }

        inputs.Add(forwardDistance);
        inputs.Add(rightDistance);
        inputs.Add(leftDistance);
        inputs.Add(rightDiagDistance);
        inputs.Add(leftDiagDistance);

        // dummy outputs, not used in CalcOutput()
        outputs.Add(0);
        outputs.Add(0);

        calcOutputs = neuralNetwork.CalcOutput(inputs, outputs);

        // map outputs back to [-1,1] range
        float translationInput = Map(-1, 1, 0, 1, (float)calcOutputs[0]);
        float rotationInput = Map(-1, 1, 0, 1, (float)calcOutputs[1]);

        translation = translationInput * speed * Time.deltaTime;
        rotation = rotationInput * rotationSpeed * Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);
    }
}
