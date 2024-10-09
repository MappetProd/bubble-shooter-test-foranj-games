using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    BaseState currentState;

    void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }

    void Update()
    {
        if (currentState != null)
            currentState.UpdateLogic();
    }

    void FixedUpdate()
    {
        if (currentState != null)
            currentState.UpdatePhysics();
    }

    protected virtual BaseState GetInitialState() => null;

    public void ChangeState(BaseState newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Enter();
    }
}
