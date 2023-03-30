using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStateBehaviour : MonoBehaviour
{
    public int priority;
    protected EnemyAgent agent;
    protected AnimatorHandler animator;
    protected NavigationController nav;

    void Awake()
    {
        agent = GetComponent<EnemyAgent>();
        animator = GetComponent<AnimatorHandler>();
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    public virtual void Initialize(NavigationController nav)
    {
        this.nav = nav;
    }

    public virtual void StartBehaviour()
    {

    }
    public virtual void StopBehaviour()
    {
        nav.Stop();
    }
    public abstract bool RunCheck();
    public abstract void UpdateBehaviour();

    protected void UpdateRotation(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);
        Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime*8f);
        transform.rotation = newRotation;
    }
}