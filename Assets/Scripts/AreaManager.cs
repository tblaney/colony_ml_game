using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

//
public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance;
    public float timer;
    public int agentNum;
    public int foodNum;
    public int poisonNum;

    //Called once only when Agent
    //is first created
    //not when resetting
    void Awake(){
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        Instance = this;
        timer = 0f;
    }

    void Update(){
        timer += Time.deltaTime;
    }

    public void EnvironmentReset()
    {
        //When resetting the environment
        //clear all foods and poison
        //then, reset every game board
        ClearObjects("Food");
        ClearObjects("Poison");

        ColonyArea[] listArea = FindObjectsOfType<ColonyArea>();
        foreach (var fa in listArea)
        {
            fa.ResetArea();
        }
    }

    void ClearObjects(string objTag){
        GameObject[] objs = GameObject.FindGameObjectsWithTag(objTag);
        foreach (GameObject o in objs){
            Destroy(o);
        }
    }

    //spawn food in a random cell 
    //in a given game board
    public void SpawnFood(int index){
        ColonyArea[] listArea = FindObjectsOfType<ColonyArea>();
        foreach (var fa in listArea)
        {
            if (fa.areaIndex == index){
                fa.CreateFood(1);
            }
        }
    }


    //spawn poison in a random cell 
    //in a given game board
    public void SpawnPoison(int index){
        ColonyArea[] listArea = FindObjectsOfType<ColonyArea>();
        foreach (var fa in listArea)
        {
            if (fa.areaIndex == index){
                fa.CreatePoison(1);
            }
        }
    }
}
