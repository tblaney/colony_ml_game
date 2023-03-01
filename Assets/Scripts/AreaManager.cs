using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance;
    public float timer;
    public int agentNum;
    public int foodNum;
    public int poisonNum;

    public Bounds dropBounds;

    public Camera overheadCamera;

    public static float RewardTotal;

    void Awake()
    {
        Instance = this;
        timer = 0f;
    }

    void Start()
    {
        //EnvironmentReset();
    }

    void OnEnable()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
    }

    void OnDisable()
    {
        //Academy.Instance.OnEnvironmentReset -= EnvironmentReset;
    }

    void Update()
    {
        timer += Time.deltaTime;
    }

    public void EnvironmentReset()
    {
        ClearObjects("Food");
        ClearObjects("Poison");

        ColonyArea[] listArea = FindObjectsOfType<ColonyArea>();
        foreach (var fa in listArea)
        {
            fa.ResetArea();
        }
    }

    void ClearObjects(string objTag)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(objTag);
        foreach (GameObject o in objs){
            Destroy(o);
        }
    }

    public Vector3 GetClosestFood(int area_index, Vector3 current_position, FoodLogic.Type type)
    {   
        ColonyArea area = GetArea(area_index);
        List<GameObject> objs = null;
        switch (type)
        {
            case FoodLogic.Type.Food:
                objs = area.GetFood();
                break;
            case FoodLogic.Type.Poison:
                objs = area.GetPoison();
                break;
        }
        Vector3 closest_position = default(Vector3);
        float distance = 10000f;
        foreach (GameObject obj in objs)
        {
            float dist = Vector3.Distance(obj.transform.position, current_position);
            if (dist < distance)
            {
                distance = dist;
                closest_position = obj.transform.position;
            }
        }
        return closest_position;
    }

    ColonyArea GetArea(int index)
    {
        ColonyArea[] listArea = FindObjectsOfType<ColonyArea>();
        foreach (var fa in listArea)
        {
            if (fa.areaIndex == index){
                return fa;
            }
        }
        return null;
    }

    public void SpawnFood(int index)
    {
        ColonyArea[] listArea = FindObjectsOfType<ColonyArea>();
        foreach (var fa in listArea)
        {
            if (fa.areaIndex == index){
                fa.CreateFood(1);
            }
        }
    }

    public void SpawnPoison(int index)
    {
        ColonyArea[] listArea = FindObjectsOfType<ColonyArea>();
        foreach (var fa in listArea)
        {
            if (fa.areaIndex == index){
                fa.CreatePoison(1);
            }
        }
    }

    public float GetRewardDrop(Vector3 position)
    {
        if (dropBounds.Contains(position))
            return 1f;
        
        return -1f;
    }
}
