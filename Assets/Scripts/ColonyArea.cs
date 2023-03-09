using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class ColonyArea : MonoBehaviour
{
    public Renderer plane;
    //public Renderer planeGoal;
    //public Renderer planeTrash;
    public GameObject food;
    public GameObject poison;
    public GameObject agent;
    public int areaIndex;

    //public Bounds dropBoundsGoal;
    //public Bounds dropBoundsTrash;

    public LayerMask mask;

    public List<Vector3> positionVecs;
    public List<Vector3> openVecs;

    public List<FoodLogic> _foods;
    public List<FoodLogic> _poisons;

    public bool _cooperative;

    public int agentNum;

    public int foodCount;
    private int foodNum;
    private int poisonNum;

    SimpleMultiAgentGroup _agentGroup;

    // Start is called before the first frame update
    void Awake()
    {
        Bounds bounds = plane.bounds;

        float xmin = bounds.min.x;
        float xmax = bounds.max.x;
        float zmin = bounds.min.z;
        float zmax = bounds.max.z;
        float y = bounds.center.y;

        //dropBoundsGoal = planeGoal.bounds;
        //d//ropBoundsGoal.extents = new Vector3(dropBoundsGoal.extents.x, 10f, dropBoundsGoal.extents.z);
        //dropBoundsTrash = planeTrash.bounds;
        //dropBoundsTrash.extents = new Vector3(dropBoundsTrash.extents.x, 10f, dropBoundsTrash.extents.z);

        for(int x=(int)xmin; x<(int)xmax; x++){
            for(int z=(int)zmin; z<(int)zmax; z++){
                positionVecs.Add(new Vector3(x+0.5f, y+0.5f, z+0.5f));
            }
        }

        PlaceAgents();
    }
    
    void Reward(float val)
    {
        if (_agentGroup != null)
            _agentGroup.AddGroupReward(val);
    }

    public Vector3 GetRandomPosition()
    {
        return positionVecs[UnityEngine.Random.Range(0, positionVecs.Count)];
    }

    public void ResetArea()
    {
        Debug.Log("Colony Area Reset");
        //If randnums is true get new random food, poison and agent nums
        ClearObjects("Food");
        ClearObjects("Poison");
        if (AreaManager.Instance.randomnums)
        {
            foodNum = Random.Range(AreaManager.Instance.minFoodNum, AreaManager.Instance.maxFoodNum);
            poisonNum = Random.Range(AreaManager.Instance.minPoisonNum, AreaManager.Instance.maxPoisonNum);
        } else {
            foodNum = AreaManager.Instance.foodNum;
            poisonNum = AreaManager.Instance.poisonNum;
        }
        CreateFood(foodNum);
        foodCount = foodNum;
        CreatePoison(poisonNum);
    }

    public void UpdateFoodCount() {
        foodCount -= 1;
        if (foodCount <= 0) {
            ResetArea();
        }
    }

    private void PlaceAgents()
    {
        if (_cooperative)
        {
            if (_agentGroup == null)
            {
                _agentGroup = new SimpleMultiAgentGroup();
            }
        }
        for (int i=0; i<agentNum; i++)
        {
            Vector3 newpos = positionVecs[UnityEngine.Random.Range(0, positionVecs.Count)];
            GameObject newAgent = Instantiate(agent, this.transform);
            newAgent.transform.position = newpos;
            System.Action<float> reward_func = Reward;
            if (!_cooperative)
                reward_func = null;
            newAgent.GetComponent<ColonyAgentSmooth>().Setup(areaIndex, this, reward_func);

            if (_cooperative)
                _agentGroup.RegisterAgent(newAgent.GetComponent<ColonyAgentSmooth>());
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

            // if (_foods == null)
            //     _foods = new List<FoodLogic>();
            
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

            
            // if (_poisons == null)
            //     _poisons = new List<FoodLogic>();
            
            _poisons.Add(foodLogic);

        }
    }

    public List<GameObject> FindChildObjectWithTag(string _tag)
     {     
        List<GameObject> retList = new List<GameObject>();
        Transform parent = transform;
        GetChildObject(parent, _tag, retList);
        return retList;
     }
 
     public void GetChildObject(Transform parent, string _tag, List<GameObject> retList)
     {
         for (int i = 0; i < parent.childCount; i++)
         {   
            
            Transform child = parent.GetChild(i);
            if (child.tag == _tag)
            {
                retList.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                GetChildObject(child, _tag, retList);
            }
         }
     }

    void ClearObjects(string objTag)
    {
        List<GameObject> objs = FindChildObjectWithTag(objTag);
        foreach (GameObject o in objs){
            Destroy(o);
        }
    }
}
