using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public class State
    {
        public string Name;
        public System.Action onFrame; //Default action to be performed (when the state is active)
        public System.Action onEnter; //When state is entered
        public System.Action onExit; //When state is exited from 

        public override string ToString()
        {
            return Name;
        }
    }

    public Dictionary<string, State> states = new Dictionary<string, State>();
    public State currentState { get; private set; }
    public State initialState;

    public State CreateState(string name)
    {
        State state = new State();
        state.Name = name;

        if(states.Count == 0)
        {
            initialState = state;
        }

        states[name] = state;

        return state;
    }


    // Update is called once per frame
    public void Update()
    {
        //No states yet
        if (states.Count == 0)
        {
            Debug.LogError("*** State machine has no states! ***");
            return;
        }

        //no current state yet
        if (currentState == null)
        {
            ChangeState(initialState);
        }

        if (currentState.onFrame != null)
        {
            currentState.onFrame();
        }
    }


    public void ChangeState(State newState)
    {
        //
        if (newState == null)
        {
            Debug.LogError("*** Can't change to a null state! ***");
            return;
        }
        //do onExit of current state
        if (currentState != null && currentState.onExit != null)
        {
            currentState.onExit();
        }


        //change to newState
        Debug.LogFormat($"*** Changing from state {currentState} to state {newState} ***");
        currentState = newState;



        // do onEnter to the newState
        if (currentState.onEnter != null)
        {
            currentState.onEnter();
        }
    }

    public void ChangeState(string newStateName)
    {
        if (states.ContainsKey(newStateName))
        {
            ChangeState(states[newStateName]);
        }
        else
        {
            Debug.LogErrorFormat($"*** State machine doesn't have the state {newStateName} ***");
            return;
        }

    }
}
