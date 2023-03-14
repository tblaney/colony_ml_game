using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ColonistStateBehaviour : MonoBehaviour
{
    // controls what the agent is doing at runtime when occupying this state
    public Colonist.State state;
    protected ColonistAgent agent;
    protected NavigationController nav;

    void Awake()
    {
        agent = GetComponent<ColonistAgent>();
        nav = GetComponent<NavigationController>();
        nav.SetSpeed(agent.colonist.traits.speed*10f);
    } 

    void OnDisable()
    {
        CancelInvoke();
    }

    public virtual void StartBehaviour()
    {

    }
    public abstract void UpdateBehaviour();

    public virtual void StopBehaviour()
    {
        nav.Stop();
    }
    
    protected void UpdateRotation(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*8f);
        transform.rotation = newRotation;
    }
}
