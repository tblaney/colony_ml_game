using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodLogic : MonoBehaviour
{
    public int areaIndex;

    public void ConsumeFood(){
        Destroy(this.gameObject);
        AreaManager.Instance.SpawnFood(areaIndex);
    }

    public void ConsumePoison(){
        Destroy(this.gameObject);
        AreaManager.Instance.SpawnPoison(areaIndex);
    }
}
