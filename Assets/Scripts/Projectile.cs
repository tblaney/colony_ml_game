using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileAgent agent;
    public EnemyAgent target;
    public float speed = 20f;
    public float damage = 10f;
    private Rigidbody rigidbody;

    void Start()
    {   
        rigidbody = this.GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.forward*speed, ForceMode.Impulse);
    }

    // Start is called before the first frame update
    public void Initialize(ProjectileAgent agent)
    {
        this.agent = agent;
        this.damage = agent.damage;
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("collision tag: " + collider.gameObject.tag);
        Debug.Log("collision layer: " + LayerMask.LayerToName(collider.gameObject.layer));
        if (collider.gameObject.tag == "Enemy")
        {
            Debug.Log("collided with enemy");
            //we've hit an enemy
            target = collider.gameObject.GetComponent<EnemyAgent>();
            if (target != null)
            {
                bool isAlive = target.Damage((int)damage);
                if (isAlive) 
                {
                    //one tenth reward for hitting enemy
                    agent.AddReward(0.1f);
                }
                else 
                {
                    //one reward for killing enemy
                    agent.AddReward(1f);
                }
                Destroy();
            }
        } else if (collider.gameObject.tag == "Agent") {
            // bullets should not hit agents
            return;
        } else if (LayerMask.LayerToName(collider.gameObject.layer) == "Obstacles") {
            //bullets should break when they hit obstacles
            Debug.Log("collided with obstacle");
            Destroy();
        }
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
