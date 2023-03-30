using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using System;
using System.Collections.Generic;
using Utils;

public class HabBotAgent : Agent
{    
    [Header("Inputs:")]
    [SerializeField] private bool heuristics;
    [Space(10)]
    [Header("Movement:")]
    public float _speedRotation = 8f;
    public float _speedMovement = 6f;
    public float _speedUp = 12f;
    public bool _cooldown;
    public Action OnAttackFunc;
    protected NavigationController nav;
    float _speed;

    //-------------------------------------------//
    public void Setup(HabBot bot, Action OnAttackFunc)
    {
        nav = GetComponent<NavigationController>();
        _speedMovement = _speedMovement*bot._traits.GetTraitVal(HabBotTrait.Type.Speed);
        this.OnAttackFunc = OnAttackFunc;
    }
    public override void Initialize()
    {

    }
    public override void CollectObservations(VectorSensor sensor)
    {

    }
    void CooldownCallback()
    {
        _cooldown = false;
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //negative reward for passage of time
        // AddReward(-0.005f);

        //------discrete-actions-------//
        var move_forward = actionBuffers.DiscreteActions[0];
        var rotate = actionBuffers.DiscreteActions[1];

        if (_cooldown)
        {
            nav.SetVelocity(Vector3.zero);
            return;
        }

        switch (rotate)
        {
            case 0:
                // no rotate
                break;
            case 1:
                // rotate right
                transform.Rotate(0f, 1f*_speedRotation, 0f, Space.World);
                break;
            case 2:
                // rotate left
                transform.Rotate(0f, -1f*_speedRotation, 0f, Space.World);
                break;
        }

        Vector3 vel = transform.forward;
        float speed = _speedMovement;
        switch (move_forward)
        {
            case 0:
                // stay still
                _speed = Mathf.Lerp(_speed, 0f, Time.fixedDeltaTime*_speedUp);
                nav.SetVelocity(vel*_speed);
                break;
            case 1:
                // move forward
                _speed = Mathf.Lerp(_speed, speed, Time.fixedDeltaTime*_speedUp);
                nav.SetVelocity(vel*_speed);
                break;
        }

    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var move_forward_out = actionsOut.DiscreteActions[0];
        var rotate_out = actionsOut.DiscreteActions[1];

        if (Input.GetKey(KeyCode.W))
        {
            move_forward_out = 1;
        } 

        if (Input.GetKey(KeyCode.D))
        {
            rotate_out = 1;
        } else if (Input.GetKey(KeyCode.A))
        {
            rotate_out = 2;
        } 

        //OnActionReceived(actionsOut);

        Debug.Log("Colony Agent Heuristic: " + move_forward_out + ", " + rotate_out);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void DestroyAgent()
    {
        Destroy(this.gameObject);
    }

    public void Damage(float damage)
    {   
        //TODO implement agent health and damage-taking
        Debug.Log("Colonist agent took " + damage.ToString() + " damage");
    }
}
