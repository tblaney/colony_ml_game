using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateFood : HabBotState
{
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
        if (_targetNode != null)
        {
            _targetNode.SetBusy(false);
        }
        Queueable queueable = HabitationHandler.Instance.GetQueuedObject(HabBot.State.CollectFood);
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
        _targetNode = HabitationHandler.Instance.GetClosestNodeObjectOfType(Node.Type.Food, transform.position);
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
                _animator.SetAnimationState("Drill", 0.2f);
                if (_effectIndex == 0)
                    _effectIndex = EffectHandler.Instance.SpawnEffect(1, transform.position + transform.forward);
                _targetNode.Damage(_controller.GetBot().GetDamage(HabBot.State.CollectFood));
                Invoke("PathCallback", _controller.GetBot().GetInteractTime(HabBot.State.CollectFood));
                _interacting = true;
                _controller.ActivateAddon(HabBotAddon.Type.Drill);
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
        float distance = GetDistanceToTarget();
        if (distance < 4f)
        {
            UpdateRotation((_targetNode.GetPosition() - transform.position).normalized);
        }
    }
}   
