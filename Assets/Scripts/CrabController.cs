//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using Unity.Burst.CompilerServices;
//using Unity.VisualScripting.Antlr3.Runtime.Tree;
//using UnityEngine.UIElements;
//using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine;

public class CrabController : MonoBehaviour
{
    //player variables
    public GameObject player;


    //crab variables
    public float health = 100;
    public float speed = 6;
    private Vector3 nextPosition;
    public Transform[] positions = new Transform[4]; //startPosition, position1, position2, position3, position4;
    
    //Radiuses of crab
    public float crabChaseRadius = 14.0f;
    public float crabAttackRadius = 10.0f;
    
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


    void RoamOnFrame()
    {
        Debug.Log("Roam.onFrame");
        Roam();

        if (SenseEnemy() && StrongEnough())
            stateMachine.ChangeState(hunt);
        else if (NoHealth())
            stateMachine.ChangeState(death);
        else if (SenseEnemy() && !StrongEnough())
            stateMachine.ChangeState(evade);
    }


    private void Roam()
    {
        //moves the crab towards the nextPosition
        this.transform.position = Vector3.MoveTowards(this.transform.position, nextPosition, (Time.deltaTime * speed));

        //decides which point the platform should move to at any given time
        if (transform.position == positions[0].position)       
            nextPosition = positions[1].position;       
        else if (transform.position == positions[1].position)        
            nextPosition = positions[2].position;        
        else if (transform.position == positions[2].position)       
            nextPosition = positions[3].position;        
        else if (transform.position == positions[3].position)      
            nextPosition = positions[0].position;
    }


    void HuntOnFrame()
    {
        Debug.Log("Hunt.onFrame");
        Hunt();

        if (!SenseEnemy())
            stateMachine.ChangeState(roam);
        else if (NoHealth())
            stateMachine.ChangeState(death);
        else if (EnemyInAttackRange() && StrongEnough())
            stateMachine.ChangeState(attack);
    }


    private void Hunt()
    {
        //moves the crab towards the enemy(player)
        this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, (Time.deltaTime * speed));
    }


    void AttackOnFrame()
    {
        Debug.Log("Attack.onFrame");
        Attack();

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


    private void Attack()
    {
        Debug.Log("Attacking!!!!!");
        anim.SetBool("isAttacking", true);
    }


    void EvadeOnFrame()
    {
        Debug.Log("Evade.onFrame");
        Evade();

        if (!SenseEnemy())       
            stateMachine.ChangeState(roam);  
        else if (NoHealth())       
            stateMachine.ChangeState(death);        
    }


    private void Evade()
    {
        //makes playe evade
        this.transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, (Time.deltaTime * -speed));
    }


    void DeathOnFrame()
    {
        Debug.Log("Death.onFrame");
        Death();
        //There is no recovering from death...
    }


    private void Death()
    {
        anim.SetBool("isDead", true);
    }


    //**condition methods**
    private bool SenseEnemy()
    {
        if (Vector3.Distance(player.transform.position, this.transform.position) < crabChaseRadius)
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


    //checks if the crab can attack
    private bool EnemyInAttackRange()
    {
        if(Vector3.Distance(player.transform.position, this.transform.position) < crabAttackRadius)
            return true;
        else
            return false;
    }


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
        Gizmos.DrawWireSphere(this.transform.position, crabChaseRadius);

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
