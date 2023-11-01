using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Drive : MonoBehaviour
{
    [SerializeField] private float visibleDistance = 200.0f;  // raycast distance
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private float rotationSpeed = 100.0f;

    private List<string> collectedTrainingData = new List<string>();
    private StreamWriter trainingDataWriter;

    private void Start()
    {
        // create a file to store our training data
        string path = Application.dataPath + "/trainingData.txt";
        trainingDataWriter = File.CreateText(path);
    }

    private void OnApplicationQuit()
    {
        foreach (string trainingData in collectedTrainingData)
        {
            trainingDataWriter.WriteLine(trainingData);
        }
        trainingDataWriter.Close();
    }

    private float Round(float x)
    {
        return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
    }

    private void Update()
    {
        float translationInput = Input.GetAxis("Vertical");
        float rotationInput = Input.GetAxis("Horizontal");

        float translation = speed * translationInput * Time.deltaTime;
        float rotation = rotationSpeed * rotationInput * Time.deltaTime;

        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        // raycast debug gizmos
        Debug.DrawRay(transform.position, transform.forward * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, -this.transform.right * visibleDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(45, Vector3.up) * -this.transform.right * visibleDistance, Color.green);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(-45, Vector3.up) * this.transform.right * visibleDistance, Color.green);

        // raycasts
        RaycastHit hit;

        // inputs start at 0 so they don't activate any neuron by default
        float forwardDistance = 0;
        float rightDistance = 0;
        float leftDistance = 0;
        float rightDiagDistance = 0;
        float leftDiagDistance = 0;

        // forward
        if (Physics.Raycast(transform.position, transform.forward, out hit, visibleDistance))
        {
            // inputs are normalized and rounded
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

        // store all the training data as a string
        string trainingData = forwardDistance + "," + rightDistance + "," + leftDistance +
            "," + rightDiagDistance + "," + leftDiagDistance + "," + Round(translationInput) + "," + Round(rotationInput);
        
        // only unique training data are added
        if (translationInput != 0 && rotationInput != 0)
        {
            if (!collectedTrainingData.Contains(trainingData))
            {
                collectedTrainingData.Add(trainingData);
            }
        }
    }
}
