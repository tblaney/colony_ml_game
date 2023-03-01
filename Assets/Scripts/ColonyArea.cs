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

    public LayerMask mask;

    public List<Vector3> positionVecs;
    public List<Vector3> openVecs;

    public List<FoodLogic> _foods;
    public List<FoodLogic> _poisons;


    // Start is called before the first frame update
    void Awake()
    {
        Bounds bounds = plane.bounds;

        float xmin = bounds.min.x;
        float xmax = bounds.max.x;
        float zmin = bounds.min.z;
        float zmax = bounds.max.z;
        float y = bounds.center.y;

        for(int x=(int)xmin; x<(int)xmax; x++){
            for(int z=(int)zmin; z<(int)zmax; z++){
                positionVecs.Add(new Vector3(x+0.5f, y+0.5f, z+0.5f));
            }
        }
    }

    public Vector3 GetRandomPosition()
    {
        return positionVecs[UnityEngine.Random.Range(0, positionVecs.Count)];
    }

    public void ResetArea()
    {
        Debug.Log("Colony Area Reset");
        PlaceAgents();
        CreateFood(AreaManager.Instance.foodNum);
        CreatePoison(AreaManager.Instance.poisonNum);
    }

    private void PlaceAgents()
    {
        for (int i=0; i<AreaManager.Instance.agentNum; i++)
        {
            Vector3 newpos = positionVecs[UnityEngine.Random.Range(0, positionVecs.Count)];
            GameObject newAgent = Instantiate(agent, this.transform);
            newAgent.transform.position = newpos;
            newAgent.GetComponent<ColonyAgent>()._index = areaIndex;
        }
    }

    public List<GameObject> GetFood()
    {
        List<GameObject> objs = new List<GameObject>();
        foreach (FoodLogic food in _foods)
        {
            objs.Add(food.gameObject);
        }
        return objs;
    } 
    
    public List<GameObject> GetPoison()
    {
        List<GameObject> objs = new List<GameObject>();
        foreach (FoodLogic food in _poisons)
        {
            objs.Add(food.gameObject);
        }
        return objs;
    }

    public void CreateFood(int amount)
    {
        openVecs = new List<Vector3>();
        for (float f = 1.1f; f >0.3f; f-=0.1f)
        {
            foreach (Vector3 vec in positionVecs)
            {
                var hit = Physics.OverlapBox(vec, new Vector3(f, f, f), Quaternion.identity, mask);
                if (hit.Length > 0)
                {
                    continue;
                } 
                openVecs.Add(vec);
            }
            if (openVecs.Count > 1)
            {
                break;
            } else
            {
                openVecs.Clear();
            }
        }

        for (int i=0; i<amount; i++)
        {
            int index = UnityEngine.Random.Range(0, openVecs.Count);
            Vector3 newpos = openVecs[index];
            openVecs.RemoveAt(index);

            GameObject newFood = Instantiate(food, this.transform);
            newFood.transform.position = newpos;
            FoodLogic foodLogic = newFood.GetComponent<FoodLogic>();
            foodLogic.Setup(areaIndex, FoodLogic.Type.Food, DestroyFoodCallback);

            if (_foods == null)
                _foods = new List<FoodLogic>();
            
            _foods.Add(foodLogic);
        }
    }

    void DestroyFoodCallback(FoodLogic food)
    {
        switch (food._type)
        {
            case FoodLogic.Type.Food:
                if (_foods.Contains(food))
                    _foods.Remove(food);
                break;
            case FoodLogic.Type.Poison:
                if (_poisons.Contains(food))
                    _poisons.Remove(food);
                break;
        }
    }

    public void CreatePoison(int amount)
    {
        openVecs = new List<Vector3>();
        for (float f = 1.1f; f >0.3f; f-=0.1f)
        {
            foreach (Vector3 vec in positionVecs)
            {
                var hit = Physics.OverlapBox(vec, new Vector3(f, f, f), Quaternion.identity, mask);
                if (hit.Length > 0)
                {
                    continue;
                } 
                openVecs.Add(vec);
            }
            if (openVecs.Count > 1)
            {
                break;
            } else
            {
                openVecs.Clear();
            }
        }
        for (int i=0; i<amount; i++)
        {
            int index = UnityEngine.Random.Range(0, openVecs.Count);
            Vector3 newpos = openVecs[index];
            openVecs.RemoveAt(index);

            GameObject newFood = Instantiate(poison, this.transform);
            newFood.transform.position = newpos;
            FoodLogic foodLogic = newFood.GetComponent<FoodLogic>();
            foodLogic.Setup(areaIndex, FoodLogic.Type.Poison, DestroyFoodCallback);

            
            if (_poisons == null)
                _poisons = new List<FoodLogic>();
            
            _poisons.Add(foodLogic);

        }
    }
}
