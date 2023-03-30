using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Random = UnityEngine.Random;
using System.Linq;
using System;
using System.Collections.Generic;
using Utils;

public class ProjectileAgent : ColonistAgent
{
    public int damage;
    public float cooldownTime;
    public GameObject projectilePrefab;

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //negative reward for passage of time
        // AddReward(-0.005f);

        //------discrete-actions-------//
        var move_forward = actionBuffers.DiscreteActions[0];
        var rotate = actionBuffers.DiscreteActions[1];
        var shoot = actionBuffers.DiscreteActions[2];

        switch (rotate)
        {
            case 0:
                // no rotate
                break;
            case 1:
                // rotate right
                transform.Rotate(0f, 1f*_speedRotation, 0f, Space.World);
                break;
            case 2:
                // rotate left
                transform.Rotate(0f, -1f*_speedRotation, 0f, Space.World);
                break;
        }

        Vector3 vel = transform.forward;
        float speed = _speedMovement;
        switch (move_forward)
        {
            case 0:
                // stay still
                _speed = Mathf.Lerp(_speed, 0f, Time.fixedDeltaTime*_speedUp);
                nav.SetVelocity(vel*_speed);
                break;
            case 1:
                // move forward
                _speed = Mathf.Lerp(_speed, speed, Time.fixedDeltaTime*_speedUp);
                nav.SetVelocity(vel*_speed);
                break;
        }
        switch (shoot)
        {
            case 0:
                Debug.Log("tried to shoot");
                //fire projectile
                ProjectileAttack();
                break;
        }
    }
    void ProjectileAttack()
    {
        if (!cooldown)
        {
            cooldown = true;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(this);
            Invoke("CooldownCallback", cooldownTime);
        }
    }
}
