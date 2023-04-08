    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UserControllerState : MonoBehaviour
{
    public UserController.State _state;
    protected UserController _controller;
    protected CameraController _cam;
    public bool _active;
    void Awake()
    {
        _controller = GetComponent<UserController>();
        _cam = GetComponent<CameraController>();
        _active = false;
        Initialize();
    }
    public virtual void Initialize()
    {

    }
    public void StartState()
    {
        _active = true;
        OnStartState();
    }
    public virtual void OnStartState()
    {

    }
    public void StopState()
    {
        _active = false;
        OnStopState();
    }
    public virtual void OnStopState()
    {

    }
    public virtual void UpdateState()
    {

    }
}
