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
    CollisionController collisionController;
    [Space(10)]
    [Header("Movement:")]
    [SerializeField] private float speedMovement = 4f;
    [SerializeField] private float speedRotation = 8f;
    [SerializeField] private float speedUp = 8f;

    bool cooldown;

    public int areaIndex;

    LayerMask mask;
    Rigidbody rb;
    float speedCache;

    //-------------------------------------------//
    public void Setup(int area_index)
    {
        areaIndex = area_index;
        collisionController.OnFoodCollisionFunc = CollectibleHit;
    }
    public override void Initialize()
    {
        mask = LayerMask.GetMask("Obstacles");
        rb = GetComponent<Rigidbody>();
        collisionController = GetComponent<CollisionController>();
    }
    public override void CollectObservations(VectorSensor sensor)
    {

    }
    void CollectibleHit(Collectible collectible)
    {
        // TODO: finish this method when agent collides (this is the flag that will be called when it attack something)
        if (cooldown)
            return;
        
        switch (collectible.type)
        {
            case Collectible.Type.Food:
                // TODO: Add food collection logic, reward
                break;
             case Collectible.Type.Mineral:
                // TODO: Add mineral collection logic, reward
                break;
        }

        cooldown = true;
        Invoke("CooldownCallback", 1f);
    }
    void CooldownCallback()
    {
        cooldown = false;
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //------discrete-actions-------//
        var move_forward = actionBuffers.DiscreteActions[0];
        var rotate = actionBuffers.DiscreteActions[1];

        AddReward(-0.005f);

        switch (rotate)
        {
            case 0:
                // no rotate
                break;
            case 1:
                // rotate right
                transform.Rotate(0f, 1f*speedRotation, 0f, Space.World);
                break;
            case 2:
                // rotate left
                transform.Rotate(0f, -1f*speedRotation, 0f, Space.World);
                break;

        }

        Vector3 vel = transform.forward;
        float speedTarget = speedMovement;
        switch (move_forward)
        {
            case 0:
                speedTarget = 0f;
                break;
        }
        speedCache = Mathf.Lerp(speedCache, speedTarget, Time.fixedDeltaTime*speedUp);
        rb.velocity = vel*speedCache;
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
}
