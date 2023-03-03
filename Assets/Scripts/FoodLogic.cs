using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FoodLogic : MonoBehaviour
{
    public int areaIndex;
    BoxCollider _collider;

    public Action<FoodLogic> OnDestroyFunc;

    bool _carrying;
    Transform _carryer;
    Vector3 _offset;

    public bool _targeted;

    public enum Type
    {
        Food,
        Poison,
    }
    public Type _type;

    LayerMask mask;

    void Awake()
    {
        _collider = GetComponent<BoxCollider>();

        mask = LayerMask.GetMask("Obstacles");
    }

    public void Setup(int index, Type type, Action<FoodLogic> callback = null)
    {
        areaIndex = index;
        _type = type;
        this.OnDestroyFunc = callback;

        //f (_type == Type.Food)
            //Activate(false);
    }

    void Update()
    {
        if (_carrying)
        {
            transform.position = _carryer.position + _offset;
        }
    }

    public void Activate(bool active = true)
    {
        this.gameObject.SetActive(active);
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
        _offset = transform.position - carryer.position;
        _carrying = true;
        _carryer = carryer;

        //_collider.isTrigger = true;
        this.gameObject.layer = 0;
    }

    public void Drop(bool in_zone = true)
    {
        _carrying = false;
        _carryer = null;
        if (in_zone)
        {
            switch (_type)
            {
                case Type.Food:
                    ConsumeFood();
                    break;
                case Type.Poison:
                    ConsumePoison();
                    break;
            }
        } else
        {
            WallCheck();
            this.gameObject.layer = 6;
            //_collider.isTrigger = false;
        }
    }

    void WallCheck()
    {
        Collider[] cols = Physics.OverlapBox(transform.position, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, mask);
        if (cols.Length > 0)
        {
            switch (_type)
            {
                case Type.Food:
                    ConsumeFood();
                    break;
                case Type.Poison:
                    ConsumePoison();
                    break;
            }
        } 
    }
}
