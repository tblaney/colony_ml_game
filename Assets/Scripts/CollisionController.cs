using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollisionController : MonoBehaviour
{
    public Action<FoodLogic> OnFoodCollisionFunc;

    private void OnCollisionEnter(Collision other) {
        FoodLogic food = other.collider.gameObject.GetComponent<FoodLogic>();
        if (food != null) {
        if (OnFoodCollisionFunc != null)
            OnFoodCollisionFunc(food);
        }
    }
}
