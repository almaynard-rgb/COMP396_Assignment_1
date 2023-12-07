using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Player values
    public int health;
    //initial check if player sprite is facing right
    public bool isFacingRight = true;
    public Rigidbody player;
    public float speed = 4.0f;
    //playerSprite
    public SpriteRenderer playerSprite;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        //start player with 100 health
        health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        //code for which side the character faces is here
        if ((isFacingRight && Input.GetAxis("Horizontal") < 0) || (!isFacingRight && Input.GetAxis("Horizontal") > 0))
        {
            FlipSprite();
        }

        //checks if the there is any movement. If so, set the running animation to true
        if((Input.GetAxis("Horizontal") < 0) || (Input.GetAxis("Horizontal") > 0) 
            || (Input.GetAxis("Vertical") < 0) || (Input.GetAxis("Vertical") > 0))
        {
            anim.SetBool("isRunning", true);
        }
        //checks if there is no movement. If so set the animation back to idle
        else if ((Input.GetAxis("Horizontal") == 0) && (Input.GetAxis("Vertical") == 0))
        {
            anim.SetBool("isRunning", false);
        }


        //Moves the player in the world
        Vector3 playerMovement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        player.velocity = new Vector3((playerMovement.x * speed * 1000 * Time.deltaTime), player.velocity.y, (playerMovement.y * speed * 1000 * Time.deltaTime));

    }

    //flips sprite based on check if player is facing right
    private void FlipSprite()
    {
        //if facing right flip (true)
        if (isFacingRight == true)
        {
            playerSprite.flipX = true;
            isFacingRight = !isFacingRight;
        }
        //if not facing right flip back (false)
        else if (isFacingRight == false)
        {
            playerSprite.flipX = false;
            isFacingRight = !isFacingRight;
        }
    }
}
