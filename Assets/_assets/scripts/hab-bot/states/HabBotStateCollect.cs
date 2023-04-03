using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateCollect : HabBotState
{
    [Header("Inputs:")]
    public HabBot.State _stateQueueable;
    public Node.Type _nodeType;
    public bool _requiresAddon;
    public HabBotAddon.Type _addonType;
    public int _effectIndexIn = 0;
    public string _animation;
    NodeObject _targetNode;
    public Vector3Int _offset;
    bool _interacting = false;
    int _effectIndex;
    public override void StartState()
    {
        _interacting = false;
        RefreshTarget();
    }
    void RefreshTarget()
    {
        _interacting = false;
        _controller.ClearAddons();

        if (_targetNode != null)
        {
            _targetNode.SetBusy(false);
        }
        Queueable queueable = HabitationHandler.Instance.GetQueuedObject(_stateQueueable);
        if (queueable != null)
        {
            _targetNode = HabitationHandler.Instance.GetNodeObject(queueable as Node);
            _targetNode.SetBusy(true);
            Vector3 dir = (transform.position - _targetNode.GetPosition()).normalized;
            dir.y = 0f;
            _offset = Utils.Tools.VectorToIntCeiling(dir);
            _nav.MoveTo(_targetNode.GetPosition() + _offset, PathCallback);
            return;
        }
        _targetNode = HabitationHandler.Instance.GetClosestNodeObjectOfType(_nodeType, transform.position);
        if (_targetNode == null)
        {
            //stop state
            _controller.GetBot().SetState((HabBot.State)0);
            return;
        } else
        {
            _animator.SetAnimationState("Grounded", 0.2f);
            Vector3 dir = (transform.position - _targetNode.GetPosition()).normalized;
            dir.y = 0f;
            _offset = Utils.Tools.VectorToIntCeiling(dir);
            _nav.MoveTo(_targetNode.GetPosition() + _offset, PathCallback);
        }
        _targetNode.SetBusy(true);
        _effectIndex = 0;
    }
    void PathCallback()
    {
        // start to damage nodeobject
        if (_targetNode == null)
        {
            StopState();
            RefreshTarget();
        } else
        {
            float distance = GetDistanceToTarget();
            if (distance < _interactionDistance)
            {
                if (_animation != "")
                    _animator.SetAnimationState(_animation, 0.2f);
                if (_effectIndex == 0 && _effectIndexIn != 0)
                    _effectIndex = EffectHandler.Instance.SpawnEffect(_effectIndexIn, transform.position + transform.forward);
                bool isAlive = _targetNode.Damage(_controller.GetBot().GetDamage(_stateQueueable));
                if (!isAlive)
                {
                    StopState();
                    RefreshTarget();
                    return;
                } else
                {
                    Invoke("PathCallback", _controller.GetBot().GetInteractTime(_stateQueueable));
                }
                _interacting = true;
                if (_requiresAddon)
                    _controller.ActivateAddon(_addonType);
            } else
            {
                StopState();
                RefreshTarget();
            }
        }
    }
    public override void StopState()
    {
        if (_effectIndex != 0)
            EffectHandler.Instance.StopEffect(_effectIndex);
        if (_targetNode != null)
            _targetNode.SetBusy(false);
        _controller.ClearAddons();
        _interacting = false;
        base.StopState();
        _effectIndex = 0;
    }
    float GetDistanceToTarget()
    {
        if (_targetNode == null)
            return 0f;
        return Vector3.Distance(transform.position, _targetNode.GetPosition() + _offset);
    }
    public override void UpdateState()
    {
        if (_targetNode == null)
        {
            StopState();
            RefreshTarget();
            return;
        }
        float distance = GetDistanceToTarget();
        if (distance < 4f)
        {
            Vector3 dir = (_targetNode.GetPosition() - transform.position).normalized;
            dir.y = 0f;
            UpdateRotation(dir);
        }
    }
}   
