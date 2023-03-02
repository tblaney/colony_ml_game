using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using System;
using System.Collections.Generic;

public class ColonyAgentSmooth : Agent
{
    [SerializeField] private CollisionController _collisionController;
    [SerializeField] private float _speedMovement = 4f;
    [SerializeField] private float _speedRotation = 8f;
    [SerializeField] private float _speedUp = 8f;
    [SerializeField] private Camera _cam;

    public int _index;
    public FoodLogic _targetFood;

    LayerMask _mask;
    Rigidbody _rb;
    float _speed;
    private EnvironmentParameters m_ResetParams;

    //-------------------------------------------//
    public void Setup(int area_index)
    {
        _index = area_index;

        _collisionController.OnFoodCollisionFunc = ConsumeFood;
    }

    public override void Initialize()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;

        _mask = LayerMask.GetMask("Obstacles");
        _rb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // add a vector observation to closest food
        if (_targetFood == null)
            RefreshTarget();

        Vector3 dir_food = (_targetFood.transform.position - transform.position).normalized;

        sensor.AddObservation(dir_food);
    }

    void ConsumeFood(FoodLogic food)
    {
        switch (food._type)
        {
            case FoodLogic.Type.Food:
                if (_targetFood != food)
                {
                    RefreshTarget();
                }
                food.ConsumeFood();
                AddReward(1f);
                break;
             case FoodLogic.Type.Poison:
                food.ConsumePoison();
                AddReward(-1f);
                break;
        }
    }

    void RefreshTarget()
    {
        _targetFood = AreaManager.Instance.GetClosestFoodLogic(_index, transform.position, FoodLogic.Type.Food);
        //_targetFood.Activate();
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //------discrete-actions-------//
        var move_forward = actionBuffers.DiscreteActions[0];
        var rotate = actionBuffers.DiscreteActions[1];

        AddReward(-0.005f);

        Debug.Log("Colony Agent Smooth: " + move_forward + ", " + rotate);
        
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
                //speed = 0f;
                _rb.velocity = vel*Mathf.Lerp(_speed, 0f, Time.fixedDeltaTime*_speedUp);
                break;
            case 1:
                // move forward
                //_rb.AddForce(vel*_speedMovement, ForceMode.Impulse);
                _rb.velocity = vel*Mathf.Lerp(_speed, speed, Time.fixedDeltaTime*_speedUp);
                break;
            //case 2:
                // move forward
                //_rb.AddForce(vel*_speedMovement, ForceMode.Impulse);
                //_rb.velocity = -vel*Mathf.Lerp(_speed, speed, Time.deltaTime*4f);
            //    break;
        }
        //_rb.velocity = vel*Mathf.Lerp(_speed, speed, Time.deltaTime*4f);
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
}
