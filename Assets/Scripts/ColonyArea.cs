using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonyArea : MonoBehaviour
{
    public Renderer plane;
    public GameObject food;
    public GameObject poison;
    public GameObject agent;
    public int areaIndex;
    public List<Vector3> positionVecs;

    void Awake(){
        plane = GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Bounds bounds = plane.bounds;
        float xmin = bounds.min.x;
        float xmax = bounds.max.x;
        float zmin = bounds.min.z;
        float zmax = bounds.max.z;
        float y = bounds.center.y;
        for(int x=(int)xmin; x<(int)xmax; x++){
            for(int z=(int)zmin; z<(int)zmax; z++){
                positionVecs.Add(new Vector3(x+0.5f, y+0.25f, z+0.5f));
            }
        }
    }

    public Vector3 GetRandomPosition(){
        return positionVecs[UnityEngine.Random.Range(0, positionVecs.Count)];
    }

    public void ResetArea(){
        PlaceAgents();
        CreateFood(AreaManager.Instance.foodNum);
        CreatePoison(AreaManager.Instance.poisonNum);
    }

    private void PlaceAgents(){
        for (int i=0; i<AreaManager.Instance.foodNum; i++){
            Vector3 newpos = positionVecs[UnityEngine.Random.Range(0, positionVecs.Count)];
            GameObject newAgent = Instantiate(agent, this.transform);
            newAgent.transform.position = newpos;
        }
    }

    public void CreateFood(int amount){
        List<Vector3> openVecs = new List<Vector3>();
        foreach (Vector3 vec in positionVecs){
                var hit = Physics.OverlapBox(vec, new Vector3(0.3f, 0.3f, 0.3f));
                if (hit.Length > 0){
                    continue;
                } 
                openVecs.Add(vec);
            }
        for (int i=0; i<amount; i++){
            int index = UnityEngine.Random.Range(0, openVecs.Count);
            Vector3 newpos = openVecs[index];
            openVecs.RemoveAt(index);
            GameObject newFood = Instantiate(food, this.transform);
            newFood.transform.position = newpos;
            FoodLogic foodLogic = newFood.GetComponent<FoodLogic>();
            foodLogic.areaIndex = areaIndex;
        }
    }

    public void CreatePoison(int amount){
        List<Vector3> openVecs = new List<Vector3>();
        foreach (Vector3 vec in positionVecs){
                var hit = Physics.OverlapBox(vec, new Vector3(0.3f, 0.3f, 0.3f));
                if (hit.Length > 0){
                    continue;
                } 
                openVecs.Add(vec);
            }
        for (int i=0; i<amount; i++){
            int index = UnityEngine.Random.Range(0, openVecs.Count);
            Vector3 newpos = openVecs[index];
            openVecs.RemoveAt(index);
            GameObject newPoison = Instantiate(poison, this.transform);
            newPoison.transform.position = newpos;
        }
    }
}
