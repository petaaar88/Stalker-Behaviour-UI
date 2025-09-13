using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class StateMachine<T>
{
    protected State<T> currentState = null;
    protected State<T> previousState = null;
    public State<T> globalState = null;

    protected T entity;

    public StateMachine(T entity){this.entity = entity;}

    public virtual void Update()
    {
        if (globalState != null)
            globalState.Update(entity);

        if (currentState != null)
            currentState.Update(entity);

    }

    public void ChangeState(State<T> newState)
    {
        if (newState == currentState)
            return;

        currentState.Exit(entity);
        previousState = currentState;
        currentState = newState;
        currentState.Enter(entity);
    }

    public void SetCurrentState(State<T> state) { this.currentState = state; }
    public void SetGlobalState(State<T> state) { this.globalState = state; }
    public void SetPreviousState(State<T> state) { this.previousState = state; }

    public State<T> GetCurrentState() { return this.currentState; }
    public State<T> GetPreviousState() { return this.previousState; }
}
