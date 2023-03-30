using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using System;
using System.Collections.Generic;
using Utils;

public class ColonistAgent : Agent
{    
    [Header("Inputs:")]
    [SerializeField] 
    private bool heuristics;
    [Space(10)]
    [Header("Movement:")]
    protected NavigationController nav;
    protected float _speedRotation = 8f;
    protected float _speedMovement= 1.2f;
    protected float _speedUp= 8f;

    public bool cooldown;

    public int areaIndex;
    protected float _speed;

    LayerMask mask;
    Rigidbody rb;

    //-------------------------------------------//
    public void Setup(int area_index)
    {
        cooldown = false;
        areaIndex = area_index;
        nav = GetComponent<NavigationController>();
        _speedMovement = _speedMovement*UnityEngine.Random.Range(0.7f, 1.15f);
    }
    public override void Initialize()
    {
        mask = LayerMask.GetMask("Obstacles");
        rb = GetComponent<Rigidbody>();
    }
    public override void CollectObservations(VectorSensor sensor)
    {

    }
    void CooldownCallback()
    {
        cooldown = false;
        Debug.Log("projectile cooldown callback");
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //negative reward for passage of time
        // AddReward(-0.005f);

        //------discrete-actions-------//
        var move_forward = actionBuffers.DiscreteActions[0];
        var rotate = actionBuffers.DiscreteActions[1];

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
        Debug.Log("Colonist agent took " + damage.ToString() + " damage");
        AddReward(-1f);
    }
}
