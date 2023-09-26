using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CrabController : MonoBehaviour
{
    //player variables
    public GameObject player;


    //crab variables
    public float health;
    public float speed = 6;
    private Vector3 nextPosition;
    public Transform[] positions = new Transform[4]; //startPosition, position1, position2, position3, position4;


    //crab states declared
    public StateMachine stateMachine;
    public StateMachine.State patrol, chase, attack, runAway, damage; //maybe add death state


    // Start is called before the first frame update
    void Start()
    {
        //new instance of StateMachine
        stateMachine = new StateMachine();


        //sets first waypoint on start()
        nextPosition = positions[0].position;


        //Use factory pattern
        //patrol state
        patrol = stateMachine.CreateState("Patrol");
        patrol.onEnter = delegate { Debug.Log("Patrol.onEnter"); };
        patrol.onExit = delegate { Debug.Log("Patrol.onExit"); };
        patrol.onFrame = PatrolOnFrame;


        //chase state
        chase = stateMachine.CreateState("Chase");
        chase.onEnter = delegate { Debug.Log("Chase.onEnter"); };
        chase.onExit = delegate { Debug.Log("Chase.onExit"); };
        chase.onFrame = ChaseOnFrame;


        //attack state
        attack = stateMachine.CreateState("Attack");
        attack.onEnter = delegate { Debug.Log("Attack.onEnter"); };
        attack.onExit = delegate { Debug.Log("Attack.onExit"); };
        attack.onFrame = AttackOnFrame;


        //runAway state
        runAway = stateMachine.CreateState("RunAway");
        runAway.onEnter = delegate { Debug.Log("RunAway.onEnter"); };
        runAway.onExit = delegate { Debug.Log("RunAway.onExit"); };
        runAway.onFrame = RunAwayOnFrame;


        //damage state
        damage = stateMachine.CreateState("Damage");
        damage.onEnter = delegate { Debug.Log("Damage.onEnter"); };
        damage.onExit = delegate { Debug.Log("Damage.onExit"); };
        damage.onFrame = DamageOnFrame;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }


    void PatrolOnFrame()
    {
        Debug.Log("Patrol.onFrame");
        Patrol();
    }

    private void Patrol()
    {
        //decides which point the platform should move to at any given time
        if (transform.position == positions[0].position)
        {
            nextPosition = positions[1].position;
        }
        else if (transform.position == positions[1].position)
        {
            nextPosition = positions[2].position;
        }
        else if (transform.position == positions[2].position)
        {
            nextPosition = positions[3].position;
        }
        else if (transform.position == positions[3].position)
        {
            nextPosition = positions[0].position;
        }

        //moves the platform
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, (speed * Time.deltaTime));
    }

    void ChaseOnFrame()
    {

    }


    void AttackOnFrame()
    {

    }


    void RunAwayOnFrame()
    {

    }


    void DamageOnFrame()
    {

    }


    //to draw lines between waypoints
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(positions[0].position, positions[1].position);
        Gizmos.DrawLine(positions[1].position, positions[2].position);
        Gizmos.DrawLine(positions[2].position, positions[3].position);
        Gizmos.DrawLine(positions[3].position, positions[0].position);
    }
}
