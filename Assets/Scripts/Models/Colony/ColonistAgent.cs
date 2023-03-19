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
    [SerializeField] private bool heuristics;
    [Space(10)]
    [Header("Movement:")]
    [SerializeField] private float speedRotation = 8f;
    public float minspeedstat;
    public float maxspeedstat;
    private float speedstat;
    protected NavigationController nav;

    public bool cooldown;

    public int areaIndex;

    LayerMask mask;
    Rigidbody rb;

    //-------------------------------------------//
    public void Setup(int area_index)
    {
        speedstat = Random.Range(minspeedstat, maxspeedstat);
        areaIndex = area_index;
        nav = GetComponent<NavigationController>();
        nav.SetSpeed(speedstat*6f);
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
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //negative reward for passage of time
        AddReward(-0.005f);

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
                transform.Rotate(0f, 1f*speedRotation*speedstat, 0f, Space.World);
                break;
            case 2:
                // rotate left
                transform.Rotate(0f, -1f*speedRotation*speedstat, 0f, Space.World);
                break;
        }

        switch (move_forward)
        {
            case 0:
                // stay still
                Vector3 zeroVel = new Vector3(0f, 0f, 0f);
                nav.SetVelocity(zeroVel);
                break;
            case 1:
                Vector3 forwardVel = transform.forward;
                nav.SetVelocity(forwardVel);
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
