using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FoodLogic : MonoBehaviour
{
    public int areaIndex;

    public Action<FoodLogic> OnDestroyFunc;

    bool _carrying;

    public enum Type
    {
        Food,
        Poison,
    }
    public Type _type;

    public void Setup(int index, Type type, Action<FoodLogic> callback = null)
    {
        areaIndex = index;
        _type = type;
        this.OnDestroyFunc = callback;
    }

    public void ConsumeFood(){
        if (OnDestroyFunc != null)
            OnDestroyFunc(this);
        Destroy(this.gameObject);
        AreaManager.Instance.SpawnFood(areaIndex);
    }

    public void ConsumePoison(){
        if (OnDestroyFunc != null)
            OnDestroyFunc(this);
        Destroy(this.gameObject);
        AreaManager.Instance.SpawnPoison(areaIndex);
    }

    public void Carry(Transform carryer)
    {

    }

    public void Drop()
    {

    }
}
