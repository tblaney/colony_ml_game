using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HabBotStateRest : HabBotState
{
    Notification _notification;
    float _timer;

    public override void StartState()
    {
        _animator.SetAnimationState("Grounded", 0.2f);

        BuiltNodeObject obj = HabitationHandler.Instance.GetClosestNodeObjectOfType(Node.Type.Building, _controller.GetBot()._position, 6) as BuiltNodeObject;
        Vector3 position = (obj._behaviour as BuiltNodeBehaviourRestMachine).GetZonePosition();
        position.y = 30f;
        _nav.MoveTo(position, PathCallback);
    }
    void PathCallback()
    {
        _nav.Stop();
    }
    public override void StopState()
    {
        base.StopState();
    }
    public override void UpdateState()
    {
        float distance = Vector3.Distance(transform.position, _nav.GetDestination());
        if (distance < 2f)
        {
            // heal
            _timer += Time.deltaTime;
            if (_timer > 1f)
            {
                _timer = 0f;
                HabBot bot = _controller.GetBot();
                bot.GetVitality("energy").Heal(5);
                if (bot.GetVitality("energy")._val > 98)
                {
                    if (bot._stateCache != HabBot.State.Rest && bot._stateCache != HabBot.State.Idle)
                    {
                        bot.SetState(bot._stateCache);
                    } else
                    {
                        bot.RandomizeState();
                    }
                }
            }
        }
    }
}   
