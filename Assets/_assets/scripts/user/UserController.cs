using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    [Header("Inputs:")]
    public CameraTargetController _cameraTargetController;
    [Header("Debug:")]
    public UserControllerState _stateCurrent;
    List<UserControllerState> _states;
    public enum State
    {
        Viewing,
        Following,
        Building,
        Zoning,
    }

    void Awake()
    {
        UserControllerState[] states = GetComponents<UserControllerState>();
        _states = new List<UserControllerState>();
        foreach (UserControllerState state in states)
        {
            _states.Add(state);
        }
    }
    void Start()
    {
        SetState(State.Viewing);
    }
    void Update()
    {
        if (_stateCurrent != null)
            _stateCurrent.UpdateState();
    }
    public void SetState(State state)
    {
        if (_stateCurrent != null)
            _stateCurrent.StopState();
        
        UserControllerState newState = GetState(state);
        _stateCurrent = newState;
        _stateCurrent.StartState();
    }
    public UserControllerState GetState(State state)
    {
        foreach (UserControllerState stateController in _states)
        {
            if (stateController._state == state)
                return stateController;
        }
        return null;
    }
}
