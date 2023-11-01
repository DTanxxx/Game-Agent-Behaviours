using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Brain : MonoBehaviour
{
    [SerializeField] private GameObject paddle;
    [SerializeField] private GameObject ball;
    [SerializeField] private float numBallsSaved = 0;
    [SerializeField] private Text score;
    [SerializeField] private string selfBackwallTag;

    public float numBallsMissed = 0;
    public bool human = false;

    private Rigidbody2D ballRb;
    private float paddleVelocityY;  // neural network's output
    private float paddleMinPosY = 8.8f;
    private float paddleMaxPosY = 17.4f;
    private float paddleMaxSpeed = 15.0f;
    private ANN neuralNetwork;

    private void Start()
    {
        // we have 6 inputs:
        // ball's x position
        // ball's y position
        // ball's x velocity
        // ball's y velocity
        // paddle's x position
        // paddle's y position

        // and we have 1 output:
        // paddle's y velocity
        neuralNetwork = new ANN(6, 1, 1, 4, 0.11);
        ballRb = ball.GetComponent<Rigidbody2D>();
    }

    private List<double> Run(double ballXPos, double ballYPos, double ballXVel, double ballYVel, double paddleXPos, double paddleYPos, double paddleYVel, bool train)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        inputs.Add(ballXPos);
        inputs.Add(ballYPos);
        inputs.Add(ballXVel);
        inputs.Add(ballYVel);
        inputs.Add(paddleXPos);
        inputs.Add(paddleYPos);

        outputs.Add(paddleYVel);

        if (train)
        {
            return neuralNetwork.Train(inputs, outputs);
        }
        else
        {
            // no training, query our neural network for outputs
            return neuralNetwork.CalcOutput(inputs, outputs);
        }
    }

    private void Update()
    {
        if (!human)
        {
            // update paddle position using neural network's output
            float posY = Mathf.Clamp(paddle.transform.position.y + (paddleVelocityY * Time.deltaTime * paddleMaxSpeed),
                paddleMinPosY, paddleMaxPosY);
            paddle.transform.position = new Vector3(paddle.transform.position.x, posY, paddle.transform.position.z);

            List<double> output = new List<double>();
            int layerMask = 1 << 9;  // back wall's layer mask
            RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, ballRb.velocity, 1000, layerMask);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("tops"))
                {
                    // reflect off top wall
                    Vector3 reflection = Vector3.Reflect(ballRb.velocity, hit.normal);
                    hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
                }

                if (hit.collider != null && hit.collider.gameObject.CompareTag(selfBackwallTag))
                {
                    // if raycast from ball hits back wall, train using appropriate inputs and desired output
                    // use the output value as paddleVelocityY
                    float dy = hit.point.y - paddle.transform.position.y;
                    output = Run(ball.transform.position.x,
                                 ball.transform.position.y,
                                 ballRb.velocity.x,
                                 ballRb.velocity.y,
                                 paddle.transform.position.x,
                                 paddle.transform.position.y,
                                 dy, true);
                    paddleVelocityY = (float)output[0];
                }
            }
            else
            {
                // raycast doesn't hit back wall, no need for paddle to move
                paddleVelocityY = 0;
            }
        }

        if (score != null)
        {
            score.text = numBallsMissed + "";
        }
    }
}
