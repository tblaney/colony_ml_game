using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabBotController : MonoBehaviour
{
    HabBot _bot;
    [SerializeField] private List<HabBotState> _states;
    HabBotState _state;
    AnimatorHandler _animator;
    NavigationController _nav;
    
    //----------------------------------------------------------//
    // setup
    public void Initialize(HabBot bot)
    {
        _animator = GetComponent<AnimatorHandler>();
        _nav = GetComponent<NavigationController>();

        this._bot = bot;
        SetupStates();
        SetupBot();
    }
    void SetupStates()
    {
        foreach (HabBotState state in _states)
        {
            state.Initialize();
        }
    }
    void SetupBot()
    {
        SetState(_bot._stateDefault);
    }

    //----------------------------------------------------------//
    // state-switcher
    void Update()
    {
        if (_state != null)
            _state.UpdateState();
        RestCheck();

        _animator.SetFloat("Speed", _nav.GetVelocity().magnitude);
    }
    public void SetState(HabBot.State state)
    {
        if (_state != null && _state._state != state)
            _state.StopState();
        
        HabBotState stateBehaviour = GetState(state);
        if (stateBehaviour != null)
        {
            _state = stateBehaviour;
            _state.StartState();
        }
        _bot._state = state;
    }
    public HabBotState GetState(HabBot.State state)
    {
        foreach (HabBotState stateBehaviour in _states)
        {
            if (stateBehaviour._state == state)
                return stateBehaviour;
        }
        return null;
    }
    public void MakeStateDecision()
    {
        SetState(_bot._stateDefault);
    }

    //----------------------------------------------------------//
    // checks
    void RestCheck()
    {
        if (_bot.GetVitality("energy")._val < 1)
        {
            SetState(HabBot.State.Rest);
        }
    }
}
