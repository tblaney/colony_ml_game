using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using System;

public class ColonyAgent : Agent
{

    const int k_NoAction = 0;  // do nothing!
    const int k_Up = 1;
    const int k_Down = 2;
    const int k_Left = 3;
    const int k_Right = 4;

    private EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }

    //TODO: Make reward cumulative across all agents. (look up SharedReward() ML agents method)
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var action = actionBuffers.DiscreteActions[0];
        var targetPos = transform.position;

        AddReward(-0.005f);

        switch (action)
        {
            case k_NoAction:
                break;
            case k_Right:
                targetPos = transform.position + new Vector3(1f, 0, 0);
                break;
            case k_Left:
                targetPos = transform.position + new Vector3(-1f, 0, 0);
                break;
            case k_Up:
                targetPos = transform.position + new Vector3(0, 0, 1f);
                break;
            case k_Down:
                targetPos = transform.position + new Vector3(0, 0, -1f);
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        Collider[] hit = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f));
        if (hit.Where(col => col.gameObject.CompareTag("Wall")).ToArray().Length == 0)
        {
            transform.position = targetPos;

            if (hit.Where(col => col.gameObject.CompareTag("Food")).ToArray().Length == 1)
            {
                //TODO: Gain reward for collecting food
                FoodLogic foodLogic = hit[0].gameObject.GetComponent<FoodLogic>();
                foodLogic.ConsumeFood();
                AddReward(1f);
            }
            else if (hit.Where(col => col.gameObject.CompareTag("Poison")).ToArray().Length == 1)
            {   
                //TODO: Gain negative reward for standing near or touching poison
                //punishment should be proportional to distance (eventually)
                FoodLogic foodLogic = hit[0].gameObject.GetComponent<FoodLogic>();
                foodLogic.ConsumePoison();
                AddReward(-1f);
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions[0];

        if (Input.GetKey(KeyCode.D))
        {
            action = k_Right;
        } else
        if (Input.GetKey(KeyCode.W))
        {
            action = k_Up;
        } else
        if (Input.GetKey(KeyCode.A))
        {
            action = k_Left;
        } else
        if (Input.GetKey(KeyCode.S))
        {
            action = k_Down;
        } else
        {
            action = 0;
        }

    }
}
