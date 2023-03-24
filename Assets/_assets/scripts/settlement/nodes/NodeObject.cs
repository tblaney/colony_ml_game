using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NodeObject : MonoBehaviour
{
    protected Action<NodeObject> OnDestroyFunc;
    protected Func<NodeObject, bool, List<NodeObject>> GetNeighboursFunc;
    public Node _node;
    public bool _surface;
    public int _health;

    public float _interactionTime = 30f;
    bool _interacting;
    HabBotController _botController;
    Action InteractCallbackFunc;

    public void Initialize(Node node, Action<NodeObject> OnDestroyFunc, Func<NodeObject, bool, List<NodeObject>> GetNeighboursFunc)
    {
        _node = node;
        _health = 100;
        this.OnDestroyFunc = OnDestroyFunc;
        this.GetNeighboursFunc = GetNeighboursFunc;
        InitializeNode();
    }
    public virtual void InitializeNode(){}
    public virtual void OnDestroyNode(){}
    public void DestroyNode()
    {
        if (OnDestroyFunc != null)
            OnDestroyFunc(this);
        
        OnDestroyNode();
        Destroy(this.gameObject);
    }
    public void SurfaceCheck()
    {
        if (Mathf.Abs(_node._position.y - NodeProcessor._boundsHeight) > 0.5f)
        {
            _surface = false;
            return;
        }
        List<NodeObject> neighbours = GetNeighboursFunc(this, true);
        if (neighbours.Count < 4)
        {
            _surface = true;
        } else
        {
            _surface = false;
        }
    }
    public virtual void Interact(HabBotController botController, Action CallbackFunc)
    {
        if (_interacting)
            return;
        _botController = botController;
        _interacting = true;
        this.InteractCallbackFunc = CallbackFunc;
        StartCoroutine(DelayedActionRealtime(_interactionTime, InteractCallback));
    }
    void InteractCallback()
    {
        _interacting = false;
        if (InteractCallbackFunc != null)
            InteractCallbackFunc();
        InteractCallbackFunc = null;
        InteractComplete();
    }
    public virtual void InteractComplete()
    {

    }
    public void Activate(bool active)
    {
        
    }
    public Vector3Int GetPosition()
    {
        return _node._position;
    }
    public Node GetNode()
    {
        return _node;
    }
    public void SetPosition(Vector3Int position)
    {
        transform.position = position;
        _node._position = position;
    }
    public List<NodeObject> GetNeighbours()
    {
        return GetNeighboursFunc(this, true);
    }
    public bool Damage(int val)
    {
        _health -= val;
        if (_health <= 0 )
        {
            DestroyNode();
            return false;
        }
        return true;
    }
    private IEnumerator DelayedActionRealtime(float time, Action OnDelayFunc)
    {
        yield return new WaitForSecondsRealtime(time);
        if (OnDelayFunc != null)
            OnDelayFunc();
    }
}
