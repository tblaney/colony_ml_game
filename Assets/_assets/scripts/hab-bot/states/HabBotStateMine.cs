using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateMine : HabBotState
{
    NodeObject _targetNode;
    bool _interacting = false;
    public override void StartState()
    {
        _interacting = false;
        RefreshTarget();
    }
    void RefreshTarget()
    {
        _targetNode = HabitationHandler.Instance.GetClosestNodeObjectOfType(Node.Type.Mineral, transform.position);
        if (_targetNode == null)
        {
            //stop state
            _controller.GetBot().SetState((HabBot.State)0);
        } else
        {
            _nav.MoveTo(_targetNode.GetPosition(), PathCallback);
        }
    }
    void PathCallback()
    {
        // start to damage nodeobject
        if (_targetNode == null)
        {

        } else
        {
            _targetNode.Interact(_controller, NodeObjectCallback);
            _interacting = true;
        }
    }
    void NodeObjectCallback()
    {
        if (!_interacting)
            return;
        
        RefreshTarget();
    }
    public override void StopState()
    {
        _interacting = false;
        base.StopState();
    }

    public override void UpdateState()
    {

    }
}   
