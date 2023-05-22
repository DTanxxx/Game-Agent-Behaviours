using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class Brain : MonoBehaviour
{
    public int DNALength = 1;
    public float timeAlive;  // elapsed time until the character falls off the red plank
    public DNA dna;
    public float distanceTravelled;

    private ThirdPersonCharacter m_Character;
    private Vector3 m_Move;  // movement direction
    private bool m_Jump;  // whether or not this character should jump
    private bool alive = true;
    private Vector3 startPosition;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("dead"))
        {
            // character dropped off the plank
            alive = false;
        }
    }

    public void Init()
    {
        // initialize DNA with the following possible gene values:
        // 0 = forward
        // 1 = back
        // 2 = left
        // 3 = right
        // 4 = jump
        // 5 = crouch
        dna = new DNA(DNALength, 6);
        m_Character = GetComponent<ThirdPersonCharacter>();
        timeAlive = 0f;
        alive = true;
        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        // read DNA
        float horizontalMovement = 0;
        float verticalMovement = 0;
        bool crouch = false;
        if (dna.GetGene(0) == 0)
        {
            // forward movement
            verticalMovement = 1;
        }
        else if (dna.GetGene(0) == 1)
        {
            // backward movement
            verticalMovement = -1;
        }
        else if (dna.GetGene(0) == 2)
        {
            // left movement
            horizontalMovement = -1;
        }
        else if (dna.GetGene(0) == 3)
        {
            // right movement
            horizontalMovement = 1;
        }
        else if (dna.GetGene(0) == 4)
        {
            // jump
            m_Jump = true;
        }
        else if (dna.GetGene(0) == 5)
        {
            // crouch
            crouch = true;
        }

        m_Move = verticalMovement * Vector3.forward + horizontalMovement * Vector3.right;
        m_Character.Move(m_Move, crouch, m_Jump);
        m_Jump = false;
        if (alive)
        {
            // character is still alive, increment timeAlive and update distanceTravelled
            timeAlive += Time.deltaTime;
            distanceTravelled = Vector3.Distance(transform.position, startPosition);
        }
    }
}
