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
}
