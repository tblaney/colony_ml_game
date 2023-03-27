using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NodeObject : MonoBehaviour
{
    protected Action<Node> OnDestroyFunc;
    protected Func<Node, bool, List<Node>> GetNeighboursFunc;
    public Node _node;
    protected NodeCaster _caster;
    public int _health;

    public float _interactionTime = 30f;
    bool _interacting;
    HabBotController _botController;
    Action InteractCallbackFunc;

    void Awake()
    {
        _caster = GetComponent<NodeCaster>();
    }

    public void Initialize(Node node, Action<Node> OnDestroyFunc, Func<Node, bool, List<Node>> GetNeighboursFunc)
    {
        _node = node;
        _health = 100;
        this.OnDestroyFunc = OnDestroyFunc;
        this.GetNeighboursFunc = GetNeighboursFunc;
        InitializeNode();
        SurfaceCheck();
    }
    public virtual void InitializeNode(){}
    public virtual void OnDestroyNode(){}
    public void DestroyNode()
    {
        if (OnDestroyFunc != null)
            OnDestroyFunc(this._node);
        
        OnDestroyNode();
        Destroy(this.gameObject);
    }
    public void SurfaceCheck()
    {
        if (Mathf.Abs(_node._position.y - NodeProcessor._boundsHeight) > 0.5f)
        {
            _node._surface = false;
            return;
        }
        if (GetNeighboursFunc == null)
        {
            _node._surface = true;
            return;
        }
        List<Node> neighbours = GetNeighboursFunc(this._node, true);
        if (neighbours.Count < 4)
        {
            _node._surface = true;
        } else
        {
            _node._surface = false;
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
        if (_node._position == default(Vector3Int))
            _node._position = Utils.Tools.VectorToInt(transform.position);
        return _node;
    }
    public void SetPosition(Vector3Int position)
    {
        transform.position = position;
        _node._position = position;
    }
    public List<Node> GetNeighbours()
    {
        return GetNeighboursFunc(this._node, true);
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
