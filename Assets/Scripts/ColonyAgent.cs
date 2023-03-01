using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using System;
using System.Collections.Generic;

public class ColonyAgent : Agent
{
    const int k_NoAction = 0;  // do nothing!
    const int k_Up = 1;
    const int k_Down = 2;
    const int k_Left = 3;
    const int k_Right = 4;

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
    public Vector3 _target;

    //public bool _camera_follow = false;


    private EnvironmentParameters m_ResetParams;

    public override void Initialize()
    {
        m_ResetParams = Academy.Instance.EnvironmentParameters;

        mask = LayerMask.GetMask("Obstacles");

        SetupCamera();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // add a vector observation to closest food
        if (_target == default(Vector3))
            _target = AreaManager.Instance.GetClosestFood(_index, transform.position);
        Vector3 dir = (_target - transform.position).normalized;

        sensor.AddObservation(dir);
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
            
        //if (cooldown_2)
        //    return;

        //_timer += Time.deltaTime/2f;
        //AddReward(-_timer);
        //AddReward(-0.001f);

        var action = actionBuffers.DiscreteActions[0];
        var targetPos = transform.position;

        bool moving = false;

        switch (action)
        {
            case k_NoAction:
                //AddReward(-0.005f);
                break;
            case k_Right:
                moving = true;
                targetPos = transform.position + new Vector3(1f, 0, 0);
                if (textureType == Texture.POV)
                    transform.localEulerAngles = new Vector3(0f, 90f, 0f);
                break;
            case k_Left:
                moving = true;
                targetPos = transform.position + new Vector3(-1f, 0, 0);
                if (textureType == Texture.POV)
                    transform.localEulerAngles = new Vector3(0f, -90f, 0f);
                break;
            case k_Up:
                moving = true;
                targetPos = transform.position + new Vector3(0, 0, 1f);
                if (textureType == Texture.POV)
                    transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                break;
            case k_Down:
                moving = true;
                targetPos = transform.position + new Vector3(0, 0, -1f);
                if (textureType == Texture.POV)
                    transform.localEulerAngles = new Vector3(0f, 180f, 0f);
                break;
            default:
                throw new ArgumentException("Invalid action value");
        }

        //if (moving)
        //{
        //    Debug.Log("Colony Agent Action Received: " + (targetPos - transform.position));
        //}

        Collider[] hit = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, mask);
        if (hit.Where(col => col.gameObject.CompareTag("Wall")).ToArray().Length == 0)
        {
            transform.position = targetPos;

            if (hit.Where(col => col.gameObject.CompareTag("Food")).ToArray().Length == 1)
            {
                //TODO: Gain reward for collecting food                
                FoodLogic foodLogic = hit[0].gameObject.GetComponent<FoodLogic>();
                foodLogic.ConsumeFood();
                _timer = 0f;
                if (Vector3.Equals(foodLogic.transform.position,_target))
                {
                    AddReward(1f);
                } else
                {
                    AddReward(1f);
                }

                _target = AreaManager.Instance.GetClosestFood(_index, transform.position);
            }
            else if (hit.Where(col => col.gameObject.CompareTag("Poison")).ToArray().Length == 1)
            {   
                //TODO: Gain negative reward for standing near or touching poison
                //punishment should be proportional to distance (eventually)
                FoodLogic foodLogic = hit[0].gameObject.GetComponent<FoodLogic>();
                foodLogic.ConsumePoison();
                AddReward(-1f);
            }
        } else
        {
            //AddReward(-0.01f);
        }

        //if (cooldown && !cooldown_2)
        //{
        //    cooldown_2 = true;
        //}//
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Debug.Log("Colony Agent Heuristic");

        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = k_NoAction;

        if (cooldown)
        {
            //actionsOut.Clear();
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
    }

    void ResetCooldown()
    {
        cooldown = false;
        //cooldown_2 = false;
    }
}
