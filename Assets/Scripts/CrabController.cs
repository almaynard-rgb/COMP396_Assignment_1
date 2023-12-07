//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using Unity.Burst.CompilerServices;
//using Unity.VisualScripting.Antlr3.Runtime.Tree;
//using UnityEngine.UIElements;
//using static UnityEngine.EventSystems.EventTrigger;
using System;
using UnityEngine;

public class CrabController : MonoBehaviour
{
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
    
    //crab states declared
    public StateMachine stateMachine;
    public StateMachine.State roam, hunt, attack, evade, death;

    // Start is called before the first frame update
    void Start()
    {
        //new instance of StateMachine
        stateMachine = new StateMachine();

        //sets first waypoint on start()
        nextPosition = positions[0].position;

        //Use factory pattern
        //roam state onEnter, onExit and onFrame calls
        roam = stateMachine.CreateState("Roam");
        roam.onEnter = delegate { Debug.Log("Roam.onEnter"); };
        roam.onExit = delegate { Debug.Log("Roam.onExit"); };
        roam.onFrame = RoamOnFrame;

        //hunt state onEnter, onExit and onFrame calls
        hunt = stateMachine.CreateState("Hunt");
        hunt.onEnter = delegate { Debug.Log("Hunt.onEnter"); };
        hunt.onExit = delegate { Debug.Log("Hunt.onExit"); };
        hunt.onFrame = HuntOnFrame;

        //attack state onEnter, onExit and onFrame calls
        attack = stateMachine.CreateState("Attack");
        attack.onEnter = delegate { Debug.Log("Attack.onEnter"); };
        attack.onExit = delegate { Debug.Log("Attack.onExit"); };
        attack.onFrame = AttackOnFrame;

        //evade state onEnter, onExit and onFrame calls
        evade = stateMachine.CreateState("Evade");
        evade.onEnter = delegate { Debug.Log("Evade.onEnter"); };
        evade.onExit = delegate { Debug.Log("Evade.onExit"); };
        evade.onFrame = EvadeOnFrame;

        //death state onEnter, onExit and onFrame calls
        death = stateMachine.CreateState("Death");
        death.onEnter = delegate { Debug.Log("Death.onEnter"); };
        death.onExit = delegate { Debug.Log("Death.onExit"); };
        death.onFrame = DeathOnFrame;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    //OnFrame for the Roam state
    void RoamOnFrame()
    {
        Debug.Log("Roam.onFrame");
        Roam();

        //transitions to other states
        if (SenseEnemy() && StrongEnough())
            stateMachine.ChangeState(hunt);
        else if (NoHealth())
            stateMachine.ChangeState(death);
        else if (SenseEnemy() && !StrongEnough())
            stateMachine.ChangeState(evade);
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


    //OnFrame for the Hunt state
    void HuntOnFrame()
    {
        Debug.Log("Hunt.onFrame");
        Hunt();

        //transitions to other states
        if (!SenseEnemy())
            stateMachine.ChangeState(roam);
        else if (NoHealth())
            stateMachine.ChangeState(death);
        else if (EnemyInAttackRange() && StrongEnough())
            stateMachine.ChangeState(attack);
        else if(!StrongEnough())
            stateMachine.ChangeState(evade);
    }

    //Hunt method (functionality)
    private void Hunt()
    {
        //moves the crab towards the enemy(player)
        this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, (Time.deltaTime * speed));
    }

    //OnFrame for the Attack state
    void AttackOnFrame()
    {
        Debug.Log("Attack.onFrame");
        Attack();

        //transitions to other states. Each exit of state also ends the attack animation
        if (!StrongEnough())
        {
            anim.SetBool("isAttacking", false);
            stateMachine.ChangeState(evade);
        }
        else if (!EnemyInAttackRange() && StrongEnough())
        {
            anim.SetBool("isAttacking", false);
            stateMachine.ChangeState(hunt);
        }
        else if (NoHealth())
        {
            anim.SetBool("isAttacking", false);
            stateMachine.ChangeState(death);
        }
    }

    //Attack method (functionality)
    private void Attack()
    {
        Debug.Log("Attacking!!!!!");
        //calls the attack animation
        anim.SetBool("isAttacking", true);
    }

    //OnFrame for the Evade state
    void EvadeOnFrame()
    {
        Debug.Log("Evade.onFrame");
        Evade();

        if (!SenseEnemy())       
            stateMachine.ChangeState(roam);  
        else if (NoHealth())       
            stateMachine.ChangeState(death);     
        else if(StrongEnough() && SenseEnemy())
            stateMachine.ChangeState(hunt);
    }

    //Evade method (functionality)
    private void Evade()
    {
        //makes player evade. Uses move towards but with a negative speed for the crab to go the other way(evade direction)
        this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, (Time.deltaTime * -speed));
    }

    //OnFrame for the Death state
    void DeathOnFrame()
    {
        Debug.Log("Death.onFrame");
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
        if(health <= 0.0f)        
            return true;        
        else      
            return false;        
    }


    //sense if enemy is close enough to attack. This is determined by the crabAttackRadius
    private bool EnemyInAttackRange()
    {
        if(Vector3.Distance(player.transform.position, this.transform.position) < crabAttackRadius)
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
        //gizmo drawing for the crab chase radius
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
