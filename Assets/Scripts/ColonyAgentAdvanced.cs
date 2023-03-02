using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using System;
using System.Collections.Generic;

public class ColonyAgentAdvanced : Agent
{
    const int k_NoAction = 0;  // do nothing!
    const int k_Up = 1;
    const int k_Down = 2;
    const int k_Left = 3;
    const int k_Right = 4;

    const int k_Pickup = 1;
    const int k_Drop = 2;

    LayerMask mask;

    bool cooldown = false;
    
    RenderTextureSensorComponent renderSensor;

    public List<Camera> cams;
    public enum Texture
    {
        OverheadFollow,
        Overhead,
        POV,
    }
    public Texture textureType;

    private float _timer;
    public int _index;

    public Vector3 _targetFood;
    public Vector3 _targetPoison;
    FoodLogic _foodHolding;

    //public bool _camera_follow = false;
    public Bounds goalBounds;
    public Bounds trashBounds;


    private EnvironmentParameters m_ResetParams;

    public void Setup(int area_index, Bounds goal_bounds, Bounds trash_bounds)
    {
        _index = area_index;
        goalBounds = goal_bounds;
        trashBounds = trash_bounds;
    }

    public override void Initialize()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;

        mask = LayerMask.GetMask("Obstacles");

        SetupCamera();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // add a vector observation to closest food
        if (_targetFood == default(Vector3) | _targetPoison == default(Vector3))
            RefreshTargets();

        Vector3 dir_food = (_targetFood - transform.position).normalized;
        Vector3 dir_poison = (_targetPoison - transform.position).normalized;

        Vector3 dir_goal = (goalBounds.center - transform.position).normalized;
        Vector3 dir_trash = (trashBounds.center - transform.position).normalized;

        sensor.AddObservation(dir_food);
        sensor.AddObservation(dir_poison);

        sensor.AddObservation(dir_goal);
        sensor.AddObservation(dir_trash);

        if (_foodHolding != null)
        {
            sensor.AddObservation(0);
        } else
        {
            sensor.AddObservation(1);
        }
    }

    void RefreshTargets()
    {
        _targetFood = AreaManager.Instance.GetClosestFood(_index, transform.position, FoodLogic.Type.Food);
        _targetPoison = AreaManager.Instance.GetClosestFood(_index, transform.position, FoodLogic.Type.Poison);
    }

    void RefreshTarget(FoodLogic.Type type)
    {
        switch (type)
        {
            case FoodLogic.Type.Food:
                _targetFood = AreaManager.Instance.GetClosestFood(_index, transform.position, FoodLogic.Type.Food);
                break;
            case FoodLogic.Type.Poison:
                _targetPoison = AreaManager.Instance.GetClosestFood(_index, transform.position, FoodLogic.Type.Poison);
                break;
        }
    }

    void SetupCamera()
    {
        int i = 0;
        int j = 0;
        switch (textureType)
        {
            case Texture.OverheadFollow:
                RenderTextureSetup(0);
                AreaManager.Instance.overheadCamera.gameObject.SetActive(false);
                break;
            case Texture.Overhead:
                j = 5;
                break;
            case Texture.POV:
                RenderTextureSetup(1);
                AreaManager.Instance.overheadCamera.gameObject.SetActive(false);
                j = 1;
                break;
        }
        Debug.Log("Agent Setup Camera: " + j);
        foreach (Camera cam in cams)
        {
            if (i == j)
            {
                i++;
                continue;
            }
            
            Debug.Log(cam);
            cam.gameObject.SetActive(false);
            i++;
        }
    }

    void RenderTextureSetup(int idx)
    {
        renderSensor = GetComponent<RenderTextureSensorComponent>();
        RenderTexture tex = new RenderTexture(24, 24, 8) {};
        tex.autoGenerateMips = false;
        tex.filterMode = FilterMode.Point;

        cams[idx].targetTexture = tex;
        renderSensor.RenderTexture = tex;
    }

    //TODO: Make reward cumulative across all agents. (look up SharedReward() ML agents method)
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var action = actionBuffers.DiscreteActions[0];
        var drop_pickup = actionBuffers.DiscreteActions[1];
        var targetPos = transform.position;
        Vector3 addition = default(Vector3);
        bool moving = false;

        switch (action)
        {
            case k_NoAction:
                break;
            case k_Right:
                moving = true;
                targetPos = transform.position + new Vector3(1f, 0, 0);
                addition = new Vector3(1f, 0, 0);
                if (textureType == Texture.POV)
                    transform.localEulerAngles = new Vector3(0f, 90f, 0f);
                break;
            case k_Left:
                moving = true;
                targetPos = transform.position + new Vector3(-1f, 0, 0);
                addition = new Vector3(-1f, 0, 0);
                if (textureType == Texture.POV)
                    transform.localEulerAngles = new Vector3(0f, -90f, 0f);
                break;
            case k_Up:
                moving = true;
                targetPos = transform.position + new Vector3(0, 0, 1f);
                addition = new Vector3(0, 0, 1f);
                if (textureType == Texture.POV)
                    transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                break;
            case k_Down:
                moving = true;
                targetPos = transform.position + new Vector3(0, 0, -1f);
                addition = new Vector3(0, 0, -1f);
                if (textureType == Texture.POV)
                    transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        Collider[] hit = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, mask);
        if (hit.Where(col => col.gameObject.CompareTag("Wall")).ToArray().Length == 0)
        {
            //transform.position = targetPos;

            if (hit.Where(col => col.gameObject.CompareTag("Food")).ToArray().Length == 1)
            {
                //TODO: Gain reward for collecting food             
                FoodLogic foodLogic = hit[0].gameObject.GetComponent<FoodLogic>();
                //foodLogic.ConsumeFood();

                //AddReward(1f);
                
                if (drop_pickup == k_Pickup)
                {
                    Carry(foodLogic);
                    RefreshTarget(FoodLogic.Type.Food);
                }
            }
            else if (hit.Where(col => col.gameObject.CompareTag("Poison")).ToArray().Length == 1)
            {   
                //TODO: Gain negative reward for standing near or touching poison
                FoodLogic foodLogic = hit[0].gameObject.GetComponent<FoodLogic>();
                //foodLogic.ConsumePoison();

                //AddReward(-1f);
                //RefreshTarget(FoodLogic.Type.Poison);

                if (drop_pickup == k_Pickup)
                {
                    Carry(foodLogic);
                    RefreshTarget(FoodLogic.Type.Poison);
                }
                
            } else
            {
                // before moving, need to confirm with the food holding too
                if (_foodHolding != null)
                {
                    Collider[] hit_2 = Physics.OverlapBox(_foodHolding.transform.position + addition, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, mask);
                    if (hit_2.Length == 0)
                    {
                        // can pass
                        transform.position = targetPos;
                    }
                } else
                {
                    transform.position = targetPos;
                }
            }
        } 

        if (drop_pickup == k_Drop)
        {
            Drop();
        }
    }

    public void Carry(FoodLogic food)
    {
        if (_foodHolding != null)
        {
            // already carrying food
            AddReward(-0.005f);
        } else
        {
            _foodHolding = food;
            _foodHolding.Carry(this.transform);
            AddReward(0.005f);
        }
    }

    public void Drop()
    {
        if (_foodHolding != null)
        {
            bool in_zone;
            float reward = GetRewardDrop(out in_zone);
            AddReward(reward);

            _foodHolding.Drop(in_zone);

            switch (_foodHolding._type)
            {
                case FoodLogic.Type.Food:
                    RefreshTarget(FoodLogic.Type.Food);
                    break;
                case FoodLogic.Type.Poison:
                    RefreshTarget(FoodLogic.Type.Poison);
                    break;
            }

            _foodHolding = null;
        } else
        {
            // tried to drop with nothing
            AddReward(-0.005f);
        }
    }

    float GetRewardDrop(out bool in_zone)
    {
        in_zone = false;
        switch (_foodHolding._type)
        {
            case FoodLogic.Type.Food:
                if (goalBounds.Contains(_foodHolding.transform.position))
                {
                    in_zone = true;
                    return 1f;
                } else if (trashBounds.Contains(_foodHolding.transform.position))
                {
                    in_zone = true;
                    return -1f;
                } else
                {
                    /*
                    float distance_to_target = Vector3.Distance(_foodHolding.transform.position, goalBounds.center);
                    if (distance_to_target > 10f)
                    {
                        return -0.5f;
                    } else
                    {
                        float reward = 1 - (distance_to_target/10f);
                        if (reward > 0.5f)
                            reward = 0.5f;
                        return reward;
                    }
                    */
                    return -0.005f;
                }
            case FoodLogic.Type.Poison:
                if (trashBounds.Contains(_foodHolding.transform.position))
                {
                    in_zone = true;
                    return 1f;
                } else if (goalBounds.Contains(_foodHolding.transform.position))
                {
                    in_zone = true;
                    return -1f;
                } else
                {
                    /*
                    float distance_to_target = Vector3.Distance(_foodHolding.transform.position, trashBounds.center);
                    if (distance_to_target > 10f)
                    {
                        return -0.5f;
                    } else
                    {
                        float reward = 1 - (distance_to_target/10f);
                        if (reward > 0.5f)
                            reward = 0.5f;
                        return reward;
                    }
                    */
                    return -0.005f;
                }
        }

        return 0f;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Colony Agent Heuristic");

        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = k_NoAction;

        if (cooldown)
        {
            return;
        }
        
        cooldown = true;
        Invoke("ResetCooldown", 0.1f);

        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = k_Right;
        }
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = k_Up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = k_Left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = k_Down;
        }
        if (Input.GetKey(KeyCode.E))
        {
            discreteActionsOut[1] = k_Pickup;
        }
        if (Input.GetKey(KeyCode.F))
        {
            discreteActionsOut[1] = k_Drop;
        }
    }

    void ResetCooldown()
    {
        cooldown = false;
        //cooldown_2 = false;
    }
}
