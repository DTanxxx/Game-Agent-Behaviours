using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Replay
{
    public List<double> states;
    public double reward;

    public Replay(double platformXRotation, double ballZPos, double ballXVel, double r)
    {
        states = new List<double>();
        states.Add(platformXRotation);
        states.Add(ballZPos);
        states.Add(ballXVel);
        reward = r;
    }
}

public class Brain : MonoBehaviour
{
    [SerializeField] private GameObject ball;

    private ANN neuralNetwork;

    private float reward = 0.0f;  // reward to associate with actions
    private List<Replay> replayMemory = new List<Replay>();  // memory - list of past actions and rewards
    private int memoryCapacity = 10000;  // memory capacity

    private float discount = 0.99f;  // how much future states affect rewards
    private float exploreRate = 100.0f;  // chance of picking random action
    private float maxExploreRate = 100.0f;  // max chance value
    private float minExploreRate = 0.01f;  // min chance value
    private float exploreDecay = 0.0001f;  // chance decay amount for each update

    private Vector3 ballStartPos;  // record start position of object
    private int failCount = 0;  // count when the ball is dropped
    private float tiltSpeed = 0.5f;  // max angle to apply to tilting each update
                                     // make sure this is large enough so that the q value multiplied
                                     // by it is enough to recover balance when the ball gets a good speed up
    private float timer = 0;  // timer to keep track of balancing
    private float maxBalanceTime = 0;  // record time ball is kept balanced

    private GUIStyle guiStyle = new GUIStyle();

    private void Start()
    {
        neuralNetwork = new ANN(3, 2, 1, 6, 0.2f);  // 2 outputs - amounts of tilt in left and right directions
        ballStartPos = ball.transform.position;
        Time.timeScale = 5.0f;
    }

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 600, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + failCount, guiStyle);
        GUI.Label(new Rect(10, 50, 500, 30), "Decay Rate: " + exploreRate, guiStyle);
        GUI.Label(new Rect(10, 75, 500, 30), "Last Best Balance: " + maxBalanceTime, guiStyle);
        GUI.Label(new Rect(10, 100, 500, 30), "This Balance: " + timer, guiStyle);
        GUI.EndGroup();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ResetBall();
        }
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;

        List<double> states = new List<double>();  // inputs to neural network
        List<double> qs = new List<double>();  // outputs (Q values) from neural network

        states.Add(transform.rotation.x);
        states.Add(ball.transform.position.z);
        states.Add(ball.GetComponent<Rigidbody>().angularVelocity.x);

        qs = SoftMax(neuralNetwork.CalcOutput(states));
        double maxQ = qs.Max();  // we will have 2 Q values to consider (tiltR, tiltL); we want to know which one is the maximum
        int maxQIndex = qs.ToList().IndexOf(maxQ);
        exploreRate = Mathf.Clamp(exploreRate - exploreDecay, minExploreRate, maxExploreRate);

        // check if we are exploring new random actions
        if (Random.Range(0, 100) < exploreRate)
        {
            maxQIndex = Random.Range(0, 2);
        }

        if (maxQIndex == 0)
        {
            // rotate rightward
            transform.Rotate(Vector3.right, tiltSpeed * (float)qs[maxQIndex]);
        }
        else if (maxQIndex == 1)
        {
            // rotate leftward
            transform.Rotate(Vector3.right, -tiltSpeed * (float)qs[maxQIndex]);
        }

        if (ball.GetComponent<BallState>().dropped)
        {
            // ball dropped, give very bad reward
            reward = -1.0f;
        }
        else
        {
            reward = 0.1f;
        }

        Replay lastMemory = new Replay(transform.rotation.x, ball.transform.position.z,
            ball.GetComponent<Rigidbody>().angularVelocity.x, reward);

        if (replayMemory.Count > memoryCapacity)
        {
            // over memory capacity, remove the first element in our memory list
            replayMemory.RemoveAt(0);
        }

        replayMemory.Add(lastMemory);

        if (ball.GetComponent<BallState>().dropped)
        {
            // train the neural network every time the ball is dropped
            // loop backwards, working with the most recent action and reward first
            for (int i = replayMemory.Count - 1; i >= 0; --i)
            {
                List<double> outputsOld = new List<double>();
                List<double> outputsNew = new List<double>();
                outputsOld = SoftMax(neuralNetwork.CalcOutput(replayMemory[i].states));

                double maxQOld = outputsOld.Max();
                int action = outputsOld.ToList().IndexOf(maxQOld);

                double feedback;
                if (i == replayMemory.Count - 1 || replayMemory[i].reward == -1)
                {
                    feedback = replayMemory[i].reward;
                }
                else
                {
                    outputsNew = SoftMax(neuralNetwork.CalcOutput(replayMemory[i + 1].states));
                    maxQ = outputsNew.Max();
                    feedback = (replayMemory[i].reward + discount * maxQ);  // Bellman formula for updating Q values
                }

                outputsOld[action] = feedback;
                neuralNetwork.Train(replayMemory[i].states, outputsOld);
            }

            if (timer > maxBalanceTime)
            {
                maxBalanceTime = timer;
            }

            timer = 0;

            ball.GetComponent<BallState>().dropped = false;
            transform.rotation = Quaternion.identity;
            ResetBall();
            replayMemory.Clear();
            failCount++;
        }
    }

    private void ResetBall()
    {
        ball.transform.position = ballStartPos;
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        ball.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
    }

    // normalize the output values
    private List<double> SoftMax(List<double> values)
    {
        double max = values.Max();

        float scale = 0.0f;
        for (int i = 0; i < values.Count; ++i)
        {
            scale += Mathf.Exp((float)(values[i] - max));
        }

        List<double> result = new List<double>();
        for (int i = 0; i < values.Count; ++i)
        {
            result.Add(Mathf.Exp((float)(values[i] - max)) / scale);
        }

        return result;
    }
}
