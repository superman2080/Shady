using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> where T : MonoBehaviour
{
    public IState<T> CurState { get => state; }
    public event Action<IState<T>, IState<T>> OnStateChanged;

    private IState<T> state;
    private T caster;
    private Dictionary<Enum, IState<T>> stateDict = new();
    private Dictionary<IState<T>, List<(Func<bool> condition, IState<T> next)>> transitionDict = new();
    private List<(Func<bool> condition, IState<T> next)> anyStateTransitions = new();

    public StateMachine(T caster)
    {
        this.caster = caster;
    }

    public StateMachine(T caster, IState<T> state)
    {
        this.caster = caster;
        this.state = state;
        this.state.Enter(caster);
    }

    public void ChangeState(IState<T> newState)
    {
        if (newState == null) return;

        var prevState = state;
        state?.Exit(caster);
        state = newState;
        state.Enter(caster);

        OnStateChanged?.Invoke(prevState, state);
    }

    public void ChangeState(Enum newState)
    {
        var prevState = state;

        if (!stateDict.TryGetValue(newState, out var toState))
            return;

        ChangeState(toState);
    }

    public void Update()
    {
        state?.Execute(caster);
        CheckNextStateCondition();
    }

    public void FixedUpdate()
    {
        state?.FixedExecute(caster);
    }

    #region State Registration

    public void RegisterState(Enum type, IState<T> state)
    {
        stateDict[type] = state;
    }

    public void UnregisterState(Enum type)
    {
        stateDict.Remove(type);
    }

    public IState<T> GetState(Enum type)
    {
        return stateDict.TryGetValue(type, out var state) ? state : null;
    }

    #endregion

    #region State Comparison

    public bool CompareState(IState<T> s1, IState<T> s2)
    {
        if (s1 == null || s2 == null) return false;
        return s1.GetType() == s2.GetType();
    }

    public bool CompareState(IState<T> s1, string s2)
    {
        return s1 != null && s2 != null && s1.GetType().Name.Equals(s2);
    }

    #endregion

    #region Transition Conditions

    public void RegisterCondition(IState<T> from, IState<T> to, Func<bool> condition)
    {
        if (from == null || to == null || condition == null) return;

        if (!transitionDict.ContainsKey(from))
            transitionDict[from] = new List<(Func<bool>, IState<T>)>();

        transitionDict[from].Add((condition, to));
    }

    public void RegisterCondition(Enum from, Enum to, Func<bool> condition)
    {
        RegisterCondition(GetState(from), GetState(to), condition);
    }

    public void UnregisterCondition(IState<T> from, IState<T> to)
    {
        if (from == null || to == null) return;

        if (transitionDict.TryGetValue(from, out var transitions))
            transitions.RemoveAll(tran => tran.next == to);
    }

    public void UnregisterCondition(Enum from, Enum to)
    {
        UnregisterCondition(GetState(from), GetState(to));
    }

    #endregion

    #region Any State Transitions

    public void RegisterAnyStateTransition(IState<T> to, Func<bool> condition)
    {
        if (to == null || condition == null) return;

        anyStateTransitions.Add((condition, to));
    }

    public void RegisterAnyStateTransition(Enum to, Func<bool> condition)
    {
        RegisterAnyStateTransition(GetState(to), condition);
    }

    public void UnregisterAnyStateTransition(IState<T> to)
    {
        if (to == null) return;

        anyStateTransitions.RemoveAll(t => t.next == to);
    }

    public void UnregisterAnyStateTransition(Enum to)
    {
        UnregisterAnyStateTransition(GetState(to));
    }

    public void ClearAnyStateTransitions()
    {
        anyStateTransitions.Clear();
    }

    #endregion

    #region Transition Check

    private void CheckNextStateCondition()
    {
        if (state == null) return;

        // Any State 전이 우선 체크
        if (CheckAnyStateTransitions())
            return;

        // 일반 전이 체크
        CheckNormalTransitions();
    }

    private bool CheckAnyStateTransitions()
    {
        foreach (var (condition, next) in anyStateTransitions)
        {
            // 현재 상태와 동일한 상태로의 전이 방지
            if (state == next) continue;

            if (condition())
            {
                ChangeState(next);
                return true;
            }
        }
        return false;
    }

    private void CheckNormalTransitions()
    {
        if (!transitionDict.TryGetValue(state, out var transitions))
            return;

        foreach (var (condition, next) in transitions)
        {
            if (condition())
            {
                ChangeState(next);
                break;
            }
        }
    }

    #endregion
}