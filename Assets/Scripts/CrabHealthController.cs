using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CrabHealthController : MonoBehaviour
{
    //variable for the Crab Health text UI element
    public Text crabHealthText;
    //to get health for the StateMachine Crab
    public CrabController crab1;
    //to get health for the SimpleFSM Crab
    public CrabFSM crab2;

       // Start is called before the first frame update
    void Start()
    {
        //Initial text on start
        crabHealthText.text = "Both Crab's Health: " + 100;
    }

    // Update is called once per frame
    void Update()
    {
        //If O key is pressed, damage the player
        if (Input.GetKey(KeyCode.O))
            Damage();
        //If P key is pressed, heal the player
        else if (Input.GetKey(KeyCode.P))
            Heal();
    }

    //heal method
    private void Heal()
    {
        //adds one health to the crab
        crab1.health = crab1.health + 1;
        //makes sure both crabs have the same health
        crab2.health = crab1.health;
        //sets the text to reflect the health of the crabs
        crabHealthText.text = "Both Crab's Health: " + crab1.health;
    }

    //damage method
    private void Damage()
    {
        //removes one health to the crab
        crab1.health = crab1.health - 1;
        //makes sure both crabs have the same health
        crab2.health = crab1.health;
        //sets the text to reflect the health of the crabs
        crabHealthText.text = "Both Crab's Health: " + crab1.health;
    }
}
