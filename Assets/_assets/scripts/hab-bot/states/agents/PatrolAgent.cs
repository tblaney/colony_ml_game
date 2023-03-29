using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAgent : HabBotAgent
{
    public int damage;
    public float hitRadius;
    public float cooldownTime;
    private EnemyAgent enemy;


    void FixedUpdate()
    {   
        // get enemy directly in front of 
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, hitRadius, LayerMask.GetMask("Agents"))){
            Debug.Log("target in front of agent");
            if (hit.transform.gameObject.tag == "Enemy")
            {
                Debug.Log("attack target found");
                enemy = hit.transform.gameObject.GetComponent<EnemyAgent>();
                if (!_cooldown)
                {
                    Debug.Log("attack is called");
                    Attack();
                }
            }
        }
    }

    void Attack()
    {
        //ensure last bumped enemy is still alive
        if (enemy != null && enemy.gameObject != null)
        {
            Debug.Log("attack hit a non-null enemy");
            //Damage enemy and set cooldown flag. Gain reward if enemy is killed.
            bool enemyAlive = enemy.Damage(damage);
            AddReward(0.10f);
            _cooldown = true;
            if (!enemyAlive)
            {
                Debug.Log("attack Enemy-killing reward assigned");
                AddReward(1f);
            }
            Invoke("CooldownCallback", cooldownTime);
        }
    }

    void CooldownCallback()
    {
        _cooldown = false;
    }
}
