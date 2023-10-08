using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabFSM : MonoBehaviour
{
    public enum CrabState
    {
        //States of the crab enemy type
        Roam, Hunt, Attack, Evade, Death
    }

    //Current state of the crab
    public CrabState currentCrabState;

    //player variables
    public GameObject player;

    //crab variables
    public float health = 100;
    public float speed = 16;
    
    //position variables
    private Vector3 nextPosition;
    public Transform[] positions = new Transform[4]; //startPosition, position1, position2, position3, position4;



    //Radiuses of crab
    public float crabHuntRadius = 20.0f;
    public float crabAttackRadius = 8.0f;

    //crab animator
    public Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        //set initial position of crab to the first position transform
        nextPosition = positions[0].position;
    }


    // Update is called once per frame
    void Update()
    {
        FSM();
    }

    //All crab states handled by a switch statement that evaluates currentState
    private void FSM()
    {
        switch(currentCrabState) 
        {
            //roam state
            case CrabState.Roam:
                HandleRoam();
                break;
            //hunt state
            case CrabState.Hunt:
                HandleHunt();
                break;
            //attack state
            case CrabState.Attack:
                HandleAttack();
                break;
            //evade state
            case CrabState.Evade:
                HandleEvade();
                break;
            //death state
            case CrabState.Death:
                HandleDeath();
                break;
            default:
                break;
        }
    }

    //Changes the crab state once called
    private void ChangeCrabState(CrabState newCrabState)
    {
        //assigns currentCrabState state to the newCrabState
        currentCrabState = newCrabState;
    }

    //Handles the Roam state
    private void HandleRoam()
    {
        print("Roaming...");
        Roam();

        //transitions to other states
        if (SenseEnemy() && StrongEnough())
            ChangeCrabState(CrabState.Hunt);
        else if (NoHealth())
            ChangeCrabState(CrabState.Death);
        else if (SenseEnemy() && !StrongEnough())
            ChangeCrabState(CrabState.Evade);
    }

    //Roam method (functionality)
    private void Roam()
    {
        //moves the crab towards the nextPosition
        this.transform.position = Vector3.MoveTowards(this.transform.position, nextPosition, (Time.deltaTime * speed));

        //decides which point the nextPoint will be and therefore where the crab will move to at any given time
        if (transform.position == positions[0].position)
            nextPosition = positions[1].position;
        else if (transform.position == positions[1].position)
            nextPosition = positions[2].position;
        else if (transform.position == positions[2].position)
            nextPosition = positions[3].position;
        else if (transform.position == positions[3].position)
            nextPosition = positions[0].position;
   
    }

    //Handles the Hunt state
    private void HandleHunt()
    {
        print("Hunting...");
        Hunt();

        //transitions to other states
        if (!SenseEnemy())
            ChangeCrabState(CrabState.Roam);
        else if (NoHealth())
            ChangeCrabState(CrabState.Death);
        else if (EnemyInAttackRange() && StrongEnough())
            ChangeCrabState(CrabState.Attack);
    }

    //Hunt method (functionality)
    private void Hunt()
    {
        //moves the crab towards the enemy(player)
        this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, (Time.deltaTime * speed));
    }


    //Handles the Attack state
    private void HandleAttack()
    {
        print("Attacking...");
        Attack();

        //transitions to other states. Each exit of state also ends the attack animation
        if (!StrongEnough())
        {
            anim.SetBool("isAttacking", false);
            ChangeCrabState(CrabState.Evade);
        }
        else if (!EnemyInAttackRange() && StrongEnough())
        {
            anim.SetBool("isAttacking", false);
            ChangeCrabState(CrabState.Hunt);
        }
        else if (NoHealth())
        {
            anim.SetBool("isAttacking", false);
            ChangeCrabState(CrabState.Death);
        }
    }

    //Attack method (functionality)
    private void Attack()
    {
        Debug.Log("Attacking!!!!!");
        //calls the attack animation
        anim.SetBool("isAttacking", true);
    }

    //Handles the Evade state
    private void HandleEvade()
    {
        print("Evading...");
        Evade();

        //transitions to other states
        if (!SenseEnemy())
            ChangeCrabState(CrabState.Roam);
        else if (NoHealth())
            ChangeCrabState(CrabState.Death);
        else if (StrongEnough() && SenseEnemy())
            ChangeCrabState(CrabState.Hunt);
    }

    //Evade method (functionality)
    private void Evade()
    {
        //makes player evade. Uses move towards but with a negative speed for the crab to go the other way(evade direction)
        this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, (Time.deltaTime * -speed));
    }

    //Handles the Death state
    private void HandleDeath()
    {
        print("The crab has died...");
        Death();
        //There is no recovering from death...
    }


    //Death method (functionality)
    private void Death()
    {
        //calls the death animation
        anim.SetBool("isDead", true);
    }


    //**condition methods**

    //sense if enemy is close enough to hunt. This is determined by the crabChaseRadius
    private bool SenseEnemy()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) < crabHuntRadius)
            return true;
        else
            return false;
    }


    //checks if crab has no health
    private bool NoHealth()
    {
        if (health <= 0.0f)
            return true;
        else
            return false;
    }


    //sense if enemy is close enough to attack. This is determined by the crabAttackRadius
    private bool EnemyInAttackRange()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) < crabAttackRadius)
            return true;
        else
            return false;
    }


    //determines if the crab is strong enough.
    //If the crab is healthier or just as healthy as the player, returns true.
    //If player is healthier, return false
    private bool StrongEnough()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (health >= playerController.health)
            return true;
        else
            return false;
    }


    //to draw lines between waypoints
    private void OnDrawGizmos()
    {
        //gizmo drawing for the crab hunt radius
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(this.transform.position, crabHuntRadius);

        //gizmo drawing for the crab attack radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, crabAttackRadius);

        //gizmo drawings for between the crab waypoints
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(positions[0].position, positions[1].position);
        Gizmos.DrawLine(positions[1].position, positions[2].position);
        Gizmos.DrawLine(positions[2].position, positions[3].position);
        Gizmos.DrawLine(positions[3].position, positions[0].position);
    }
}
