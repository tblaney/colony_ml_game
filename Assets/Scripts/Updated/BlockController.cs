using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public int health;

    public void Damage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            DestroyBlock();
        }
    }

    void DestroyBlock()
    {
        
        Destroy(this.gameObject);
    }


}
