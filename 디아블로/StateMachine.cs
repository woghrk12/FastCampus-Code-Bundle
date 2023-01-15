using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T>
{
    protected StateMachine<T> stateMachine;
    protected T context;

    public State()
    { 
    
    }

    internal void SetStateMachineAndContext(StateMachine<T> p_stateMachine, T p_context)
    {
        this.stateMachine = p_stateMachine;
        this.context = p_context;

        OnInitialized();
    }

    public virtual void OnInitialized()
    { 
    
    }

    public virtual void OnEnter()
    { 
    
    }

    public abstract void OnUpdate(float p_deltaTime);

    public virtual void OnExit()
    { 
    
    }

}

public sealed class StateMachine<T>
{
    private T context;

    private State<T> curState;
    public State<T> CurState => curState;

    private State<T> priviousState;
    public State<T> PriviousState => priviousState;

    private float elapsedTimeInState = 0.0f;
    public float ElapsedTimeInState => elapsedTimeInState;

    private Dictionary<System.Type, State<T>> states = new Dictionary<System.Type, State<T>>();

    public StateMachine(T p_context, State<T> p_initialState)
    {
        this.context = p_context;
        AddState(p_initialState);
        curState = p_initialState;
        curState.OnEnter();
    }

    public void AddState(State<T> p_state)
    {
        p_state.SetStateMachineAndContext(this, context);
        states[p_state.GetType()] = p_state;
    }

    public void Update(float p_deltaTime)
    {
        elapsedTimeInState += p_deltaTime;
        curState.OnUpdate(p_deltaTime);
    }

    public R ChangeState<R>() where R : State<T>
    {
        var t_newType = typeof(R);
        if (curState.GetType() == t_newType) return curState as R;
        if (!(curState is null)) curState.OnExit();

        priviousState = curState;
        curState = states[t_newType];
        curState.OnEnter();
        elapsedTimeInState = 0.0f;

        return curState as R;
    }
}
