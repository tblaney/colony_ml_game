    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UserControllerState : MonoBehaviour
{
    public UserController.State _state;
    protected UserController _controller;
    protected CameraController _cam;
    void Awake()
    {
        _controller = GetComponent<UserController>();
        _cam = GetComponent<CameraController>();
        Initialize();
    }
    public virtual void Initialize()
    {

    }
    public void StartState()
    {
        OnStartState();
    }
    public virtual void OnStartState()
    {

    }
    public void StopState()
    {
        OnStopState();
    }
    public virtual void OnStopState()
    {

    }
    public virtual void UpdateState()
    {

    }
}
