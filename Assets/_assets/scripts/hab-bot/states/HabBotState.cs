using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class HabBotState : MonoBehaviour
{
    // controls what the _controller is doing at runtime when occupying this _state
    [Header("Inputs:")]
    public HabBot.State _state;
    public float _interactionDistance = 2.5f;
    // cache:
    protected HabBotController _controller;
    protected NavigationController _nav;
    protected AnimatorHandler _animator;

    void Awake()
    {
        _controller = GetComponent<HabBotController>();
        _nav = GetComponent<NavigationController>();
        _animator = GetComponent<AnimatorHandler>();
        Initialize();
    } 
    public virtual void Initialize()
    {

    }
    void OnDisable()
    {
        CancelInvoke();
    }
    public virtual void StartState()
    {

    }
    public abstract void UpdateState();
    public virtual void StopState()
    {
        CancelInvoke();
        _nav.Stop();
    }
    protected void UpdateRotation(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*8f);
        transform.rotation = newRotation;
    }
}   
