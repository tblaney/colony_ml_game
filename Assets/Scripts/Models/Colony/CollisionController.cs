using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollisionController : MonoBehaviour
{
    public Action<Collectible> OnFoodCollisionFunc;

    private void OnCollisionEnter(Collision other) 
    {
        Collectible food = other.collider.gameObject.GetComponent<Collectible>();
        if (food != null) {
        if (OnFoodCollisionFunc != null)
            OnFoodCollisionFunc(food);
        }
    }
}
