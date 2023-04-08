using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStatePatrol : HabBotState
{
    HabBotAgent _agent;

    public override void Initialize()
    {
        _agent = GetComponent<HabBotAgent>();
    }
    public override void StartState()
    {
        _agent.Setup(_controller.GetBot(), AttackCallback);
    }
    void AttackCallback()
    {
        // for anim basically
        _controller.ActivateAddon(HabBotAddon.Type.Sword);
        _animator.SetAnimationState("Sword Swing", 0.1f);
        AnimatorEvent animatorEvent = new AnimatorEvent("Sword Swing", AnimCallback, 0.95f);
        _animator.AddAnimatorEvent(animatorEvent);
    }
    void AnimCallback()
    {
        _animator.SetAnimationState("Grounded", 0.2f);
        _controller.ClearAddons();
    }
    public override void StopState()
    {
        base.StopState();
    }

    public override void UpdateState()
    {

    }
}
