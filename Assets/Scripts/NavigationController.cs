using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class NavigationController : MonoBehaviour
{
    NavMeshAgent navAgent;
    public Action OnDestinationFunc;
    public Vector3 destination;
    public float speed;
    public bool pathing;

    public NavMeshPathStatus pathStatus;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.updateRotation = true;
        speed = navAgent.speed;
    }

    void Update()
    {
        if (navAgent.speed != speed){
            Debug.Log("Old speed " + navAgent.speed.ToString());
            navAgent.speed = speed;
            Debug.Log("New speed " + navAgent.speed.ToString());
        }
        if (navAgent.hasPath)
            pathStatus = navAgent.pathStatus;
        else
            pathStatus = NavMeshPathStatus.PathInvalid;

        if (!pathing)
            return;

        if (ReachedDestinationOrGaveUp())
        {
            pathing = false;

            if (OnDestinationFunc != null)
                OnDestinationFunc();
        }
    }

    public void SetDestination(Vector3 destination) 
    {
        navAgent.SetDestination(destination);
        this.destination = navAgent.destination;
        pathing = true;
    }

    public void SetSpeed(float newSpeed)
    {
        this.speed = newSpeed;
    }

    public void MoveTo(Vector3 target, Action destinationFunc)
    {
        OnDestinationFunc = destinationFunc;
        if (pathing)
        {
            // refreshing an existing path, so hard calculate it to prevent slowing down
            NavMeshPath path = new NavMeshPath();
            bool has_path = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!has_path)
                SetDestination(target);
            else
            {
                navAgent.path = path;
                destination = path.corners[path.corners.Length-1];
            }
        } else
        {
            SetDestination(target);
        }
    }

    public void MoveToRandomLocation(float radius, Action destinationFunc)
    {
        OnDestinationFunc = destinationFunc;
        Vector2 randomDir = UnityEngine.Random.insideUnitCircle;
        Vector3 targetPosition = transform.position + new Vector3(randomDir.x, 0f, randomDir.y)*radius;
        SetDestination(targetPosition);
    }

    public Vector3 GetNearestPointOnNavMesh(Vector3 point)
    {
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(point, out hit, 50f, UnityEngine.AI.NavMesh.AllAreas)) {
            Vector3 result = hit.position;
            return result;
        }

        return default(Vector3);
    }


    public Vector3 GetDesiredVelocity() 
    {
        return navAgent.desiredVelocity;
    }

    public Vector3 GetVelocity()
    {
        return navAgent.velocity;
    }

    public void Stop() 
    {
        navAgent.Stop();
        navAgent.ResetPath();
        navAgent.velocity = Vector3.zero;
    }

    public void Enable(bool active)
    {
        navAgent.enabled = active;
    }

    public void SetVelocity(Vector3 velocity)
    {
        navAgent.velocity = velocity*10f;
    }

    public Vector3[] GetPathPoints()
    {
        return navAgent.path.corners;
    }

    public bool IsPathComplete()
    {
        switch (navAgent.path.status)
        {
            case NavMeshPathStatus.PathComplete:
                return true;
        }

        return false;
    }

    public bool ReachedDestinationOrGaveUp()
    {
        if (!navAgent.pathPending)
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            } else if (navAgent.pathStatus == NavMeshPathStatus.PathPartial | navAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                if (navAgent.velocity.sqrMagnitude == 0f)
                    return true;
            }
        }
        return false;
    }

}