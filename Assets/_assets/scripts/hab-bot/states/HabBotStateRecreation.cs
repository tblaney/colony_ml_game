using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using System;

public class HabBotStateRecreation : HabBotState
{
    [Header("Inputs:")]
    public VolleyballAgent _agent;
    public PhysicMaterial _physicsMaterial;
    public Transform _rotator;
    BuiltNodeBehaviourVolleyball _volleyball;
    Vector3 _targetPosition;
    bool _playing = false;
    Team _team;
    public override void StartState()
    {
         _animator.SetAnimationState("Grounded", 0.2f);

        BuiltNodeObject obj = HabitationHandler.Instance.GetClosestNodeObjectOfType(Node.Type.Building, _controller.GetBot()._position, 22) as BuiltNodeObject;
        if (obj == null)
        {
            _controller.SetState(HabBot.State.Idle);
            return;
        }
         _volleyball = (obj._behaviour as BuiltNodeBehaviourVolleyball);
        Vector3 position = _volleyball.QueueAgent(_agent, out _team);
        if (position != default(Vector3))
        {
            position.y = 30f;
            _nav.MoveTo(position, PathCallback);
            _targetPosition = position;
        } else
        {
            _controller.SetState(HabBot.State.Idle);
            return;
        }
        _playing = false;
    }
    void PathCallback()
    {
        _nav.Stop();
    }
    void PhysicsSwitch(bool isNav)
    {
        if (isNav)
        {
            if (GetComponent<Rigidbody>() != null)
            {
                Destroy(GetComponent<Rigidbody>());
                Collider col = GetComponent<Collider>();
                col.isTrigger = true;
            }
            _nav.Enable(true);
        } else
        {
            if (GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = this.gameObject.AddComponent<Rigidbody>();
                rb.mass = 50;
                rb.drag = 3;
                rb.angularDrag = 0.05f;
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.interpolation = RigidbodyInterpolation.None;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                Collider col = GetComponent<Collider>();
                col.material = _physicsMaterial;
                col.isTrigger = false;
            }   
            _nav.Enable(false);
        }
    }
    public void StartGame()
    {
        if (_team == Team.Blue)
        {
            _rotator.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        _playing = true;
        _agent.Stop();
        PhysicsSwitch(false);
    }
    public void StopGame()
    {
        if (_team == Team.Blue)
        {
            _rotator.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        _playing = false;
        PhysicsSwitch(true);
    }
    public override void StopState()
    {
        PhysicsSwitch(true);
        if (_volleyball != null)
            _volleyball.UnreadyAgent(_agent);
        base.StopState();
    }
    public override void UpdateState()
    {
        if (_playing)
            return;
        
        float distance = Vector3.Distance(transform.position, _targetPosition);
        int i = 1;
        if (distance < 4f)
        {
            // start rotating right direction
            UpdateRotation(Vector3.forward*i);
            if (distance < 1f && Vector3.Dot(transform.forward, Vector3.forward*i) > 0.95f)
            {
                _volleyball.AgentReady(_agent, StartGame, StopGame);
            }
        }
    }
}   
