using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAgent : MonoBehaviour
{
    public int areaIndex = 0;
    public float damageTimer;
    public float damageDelay;
    public float health;
    public float speed;
    public float damage;
    public EnemyState state;

    public enum EnemyState
    {
        Attack,
    }

    void Update()
    {
        damageTimer += Time.deltaTime;
        if (state == EnemyState.Attack){
            //TODO: move to the nearest colonist.
        }
        
    }

    private void OnCollisionEnter(Collision other) {
        if (state == EnemyState.Attack) {
            ColonistAgent agent = other.collider.gameObject.GetComponent<ColonistAgent>();
            if (agent != null && damageTimer > damageDelay) {
                damageTimer = 0f;
                agent.Damage(damage);
            }
        }
    }

    public void Setup(int index, float health, float speed, float damage)
    {
        this.health = health;
        this.speed = speed;
        this.damage = damage;
        areaIndex = index;
        state = EnemyState.Attack;
        damageTimer = 0f;
    }
}
