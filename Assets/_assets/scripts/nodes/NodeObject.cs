using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class NodeObject : MonoBehaviour
{
    protected Action<Node> OnDestroyFunc;
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

    public void Initialize(Node node, Action<Node> OnDestroyFunc)
    {
        _node = node;
        _health = 100;
        this.OnDestroyFunc = OnDestroyFunc;

        InitializeNode();
        _node.SurfaceCheck();
        _node.OnPositionChange += Node_PositionChange;
    }
    void OnDisable()
    {
        if (_node == null)
            return;

        _node.OnPositionChange -= Node_PositionChange;
    }
    void Node_PositionChange(object sender, EventArgs e)
    {
        transform.position = _node._position;
    }
    public virtual void InitializeNode(){}
    public virtual void OnDestroyNode(){}
    public void DestroyNode()
    {
        if (OnDestroyFunc != null)
            OnDestroyFunc(this._node);
        
        _node.DestroyNeighbourChecks();
        OnDestroyNode();
        Destroy(this.gameObject);
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
