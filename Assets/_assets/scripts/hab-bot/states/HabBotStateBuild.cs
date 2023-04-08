using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateBuild : HabBotState
{
    Node _targetNode;
    int _effectIndex;


    public override void StartState()
    {
        Queueable queueable = HabitationHandler.Instance.GetQueuedObject(HabBot.State.Build);
        if (queueable == null)
        {
            _controller.SetState((int)HabBot.State.Idle);
            return;
        } else
        {
            _targetNode = queueable as Node;
            _nav.MoveTo(_targetNode.GetPosition(), PathCallback);
        }
    }
    void PathCallback()
    {
        float distance = Vector3.Distance(transform.position, _targetNode.GetPosition());
        if (distance < _interactionDistance)
        {
            _controller.ActivateAddon(HabBotAddon.Type.Welder);
            if (_effectIndex == 0)
                    _effectIndex = EffectHandler.Instance.SpawnEffect(1, transform.position + transform.forward);
            
            Invoke("BuildCompleteCallback", _controller.GetBot().GetInteractTime(HabBot.State.Build));
        } else
        {
            HabitationHandler.Instance.AddObjectToQueue(HabBot.State.Build, _targetNode as Queueable);
            StartState();
        }
    }
    void BuildCompleteCallback()
    {
        BuildingNodeObject builder = HabitationHandler.Instance.GetNodeObject(_targetNode) as BuildingNodeObject;
        if (builder != null)
        {
            builder.FinishBuild();
            StartState();
            return;
        } else
        {
            HabitationHandler.Instance.AddObjectToQueue(HabBot.State.Build, _targetNode as Queueable);
            StartState();
        }
    }
    public override void StopState()
    {
        if (_effectIndex != 0)
            EffectHandler.Instance.StopEffect(_effectIndex);
        _controller.ClearAddons();
        _effectIndex = 0;
        base.StopState();
    }
    public override void UpdateState()
    {
        if (_targetNode == null)
            return;

        float distance = Vector3.Distance(transform.position, _targetNode.GetPosition());
        if (distance < 5f)
        {
            UpdateRotation((_targetNode.GetPosition() - transform.position).normalized);
        }
    }
}   
