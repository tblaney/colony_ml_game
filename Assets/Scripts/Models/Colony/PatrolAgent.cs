using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAgent : ColonistAgent
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
                enemy = hit.transform.gameObject.GetComponent<EnemyAgent>();
            }
            //Set enemy and attack if not on cooldown.
            if (!cooldown && enemy != null)
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        //ensure last bumped enemy is still alive
        if (enemy != null && enemy.gameObject != null)
        {
            //Damage enemy and set cooldown flag. Gain reward if enemy is killed.
            bool enemyAlive = enemy.Damage(damage);
            cooldown = true;
            if (!enemyAlive)
            {
                Debug.Log("Enemy-killing reward assigned");
                AddReward(1f);
            }
            Invoke("CooldownCallback", cooldownTime);
        }
    }

    void CooldownCallback()
    {
        cooldown = false;
    }
}
