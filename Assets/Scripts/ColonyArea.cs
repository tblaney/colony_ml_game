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
        // Divide the game board into a 20x20 grid and store
        // the cell coordinates in positionVecs
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
        //Get the coordinates of a random cell in the grid
        return positionVecs[UnityEngine.Random.Range(0, positionVecs.Count)];
    }

    public void ResetArea(){
        //Called when the environment resets
        PlaceAgents();
        CreateFood(AreaManager.Instance.foodNum);
        CreatePoison(AreaManager.Instance.poisonNum);
    }

    private void PlaceAgents(){
        //Place agent in a random cell in the grid
        for (int i=0; i<AreaManager.Instance.foodNum; i++){
            Vector3 newpos = positionVecs[UnityEngine.Random.Range(0, positionVecs.Count)];
            GameObject newAgent = Instantiate(agent, this.transform);
            newAgent.transform.position = newpos;
        }
    }

    //Places food on the grid
    public void CreateFood(int amount){

        //Get a list of cells where food can be placed in openVecs
        //i.e., where there isn't an agent or a wall

        List<Vector3> openVecs = new List<Vector3>();

        foreach (Vector3 vec in positionVecs){
                var hit = Physics.OverlapBox(vec, new Vector3(0.3f, 0.3f, 0.3f));
                if (hit.Length > 0){
                    continue;
                } 
                openVecs.Add(vec);
            }

        //Place "amount" food particles on the grid
        //Set their area index (game board) to the current one
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

    //Places poison on the grid
    public void CreatePoison(int amount){

        //Get a list of cells where poison can be placed in openVecs
        //i.e., where there isn't an agent or a wall

        List<Vector3> openVecs = new List<Vector3>();
        foreach (Vector3 vec in positionVecs){
                var hit = Physics.OverlapBox(vec, new Vector3(0.3f, 0.3f, 0.3f));
                if (hit.Length > 0){
                    continue;
                } 
                openVecs.Add(vec);
            }

        //Place "amount" poison particles on the grid
        //Set their area index (game board) to the current one
        for (int i=0; i<amount; i++){
            int index = UnityEngine.Random.Range(0, openVecs.Count);
            Vector3 newpos = openVecs[index];
            openVecs.RemoveAt(index);
            GameObject newPoison = Instantiate(poison, this.transform);
            newPoison.transform.position = newpos;
        }
    }
}
