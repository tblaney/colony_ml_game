using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodLogic : MonoBehaviour
{
    //Multiple game boards are used
    //to train multiple instances of the agent
    //in parallel. areaIndex is used to identify
    //a game board
    public int areaIndex;

    //When agent consumes food, 
    //spawn a new piece of food
    //somewhere in the grid
    public void ConsumeFood(){
        Destroy(this.gameObject);
        AreaManager.Instance.SpawnFood(areaIndex);
    }

    //When agent consumes poison, 
    //spawn a new piece of poison
    //somewhere in the grid
    public void ConsumePoison(){
        Destroy(this.gameObject);
        AreaManager.Instance.SpawnPoison(areaIndex);
    }
}
