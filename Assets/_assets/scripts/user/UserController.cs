using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserController : MonoBehaviour
{
    [Header("Inputs:")]
    public CameraTargetController _cameraTargetController;
    public SelectorController _selector;
    [Header("Debug:")]
    public UserControllerState _stateCurrent;
    List<UserControllerState> _states;
    public enum State
    {
        Viewing,
        Following,
        Building,
        Move,
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
        if (_stateCurrent == null)
        {
            SetState(UserController.State.Viewing);
            return;
        }
        if (_stateCurrent != null)
        {
            _stateCurrent.UpdateState();
            if (!_stateCurrent._active)
                _stateCurrent = null;
        }
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
    public void ActivateSelector(bool active = true)
    {
        _selector.Activate(active);
    }
    public void UpdateSelector(Vector3Int position, SelectorController.State state)
    {
        _selector.UpdateSelector(position, state);
    }
}
