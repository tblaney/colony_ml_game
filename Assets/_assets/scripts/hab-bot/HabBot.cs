using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HabBot 
{
    public string _name;
    public int _index;
    public enum State
    {
        Idle,
        Rest,
        Roam,
        CollectMinerals,
        CollectTrees,
        CollectFood,
        Farm,
        Build,
        Patrol,
        Rescue,
        Machining,
        Stockpile,
        Path,
        Recreation,
        Heal,
    }
    public State _state;
    public State _stateDefault;
    public List<Vitality> _vitalities;
    public HabBotTraits _traits;
    public List<HabBotAddon> _addons;
    public ItemInventory _inventory;

    // events
    public event EventHandler<StateChangeEventArgs> OnStateChange;
    public event EventHandler OnDeath;
    public class StateChangeEventArgs : EventArgs
    {
        public HabBot _bot;
    }

    public void InitializeRandom(int index)
    {
        _name = "00" + index.ToString();
        _index = index;
        _traits = new HabBotTraits();
        _traits.InitializeRandom();
        DetermineDefaultState();
        _vitalities = new List<Vitality>();
        for (int i = 0; i < 4; i++)
        {
            Vitality vitality = new Vitality();
            switch (i)
            {
                case 0:
                    vitality._name = "health";
                    vitality._val = 100;
                    break;
                case 1:
                    vitality._name = "energy";
                    vitality._val = 100;
                    break;
                case 2:
                    vitality._name = "hunger";
                    vitality._val = 100;
                    break;
            }
            _vitalities.Add(vitality);
        }
    }
    public void DetermineDefaultState()
    {
        _stateDefault = _traits.GetDefaultState();
    }
    public void SetState(State state)
    {
        // everything related to bot switching states is executed from this function (state change event)
        _state = state;
        StateChangeEventArgs eventArgs = new StateChangeEventArgs() {_bot = this};
        OnStateChange?.Invoke(null, eventArgs);
    }
    public void StateFailure()
    {
        // invoked when the agent cant fulfill its current target
        SetState((State)0);
    }
    public List<State> GetAvailableStates()
    {
        List<State> states = new List<State>();
        for (int i = 0; i < 15; i++)
        {
            State state = (State)i;
            if (TestStateAvailability(state))
                states.Add(state);
        }
        return states;
    }
    bool TestStateAvailability(State state)
    {
        HabBotAddon addon = null;
        switch (state)
        {
            default: return true;
            case State.CollectMinerals:
                addon = GetAddon(HabBotAddon.Type.Drill);
                if (addon != null)
                    return true;
                return false;
            case State.CollectTrees:
                addon = GetAddon(HabBotAddon.Type.Axe);
                if (addon != null)
                    return true;
                return false;
            case State.Farm:
                addon = GetAddon(HabBotAddon.Type.FarmTool);
                if (addon != null)
                    return true;
                return false;
            case State.Build:
                addon = GetAddon(HabBotAddon.Type.Welder);
                if (addon != null)
                    return true;
                return false;
            case State.Patrol:
                addon = GetAddon(HabBotAddon.Type.Sword);
                if (addon != null)
                    return true;
                return false;
            case State.Rescue:
                bool botInjured = HabitationHandler.Instance.IsBotInjured();
                return botInjured;
        }
        return false;
    }
    public HabBotAddon GetAddon(HabBotAddon.Type type)
    {
        if (_addons == null)
            return null;
        
        foreach (HabBotAddon addon in _addons)
        {
            if (addon._type == type)
                return addon;
        }
        return null;
    }
    public Vitality GetVitality(string name)
    {
        foreach (Vitality vitality in _vitalities)
        {
            if (vitality._name == name)
                return vitality;
        }
        return null;
    }
}
[Serializable]
public class HabBotTraits
{
    public List<HabBotTrait> _traits;
    public int _colorIndex;
    public event EventHandler OnTraitsChange;
    public void InitializeRandom()
    {
        _traits = new List<HabBotTrait>();
        int i = 0;
        foreach(HabBotTrait.Type traitType in Enum.GetValues(typeof(HabBotTrait.Type)))
        {
            float val = UnityEngine.Random.Range(0.1f, 0.9f);
            bool editable = false;
            if (i >= 4)
            {
                editable = true;
            }
            HabBotTrait trait = new HabBotTrait(i, traitType, val, editable);
            _traits.Add(trait);
            i++;
        }
    }
    public HabBot.State GetDefaultState()
    {
        HabBotTrait definingTrait = default(HabBotTrait);
        float max = 0f;
        foreach (HabBotTrait trait in _traits)
        {
            if (trait._editable)
                continue;
            
            if (trait._val > max)
            {
                max = trait._val;
                definingTrait = trait;
            }
        }
        List<HabBot.State> possibleStates = new List<HabBot.State>();
        switch (definingTrait._type)
        {
            case HabBotTrait.Type.Laziness:
                possibleStates.Add(HabBot.State.Roam);
                break;
            case HabBotTrait.Type.Strength:
                possibleStates.Add(HabBot.State.CollectMinerals);
                possibleStates.Add(HabBot.State.Patrol);
                possibleStates.Add(HabBot.State.Rescue);
                break;
            case HabBotTrait.Type.Speed:
                possibleStates.Add(HabBot.State.CollectFood);
                possibleStates.Add(HabBot.State.Heal);
                possibleStates.Add(HabBot.State.Rescue);
                possibleStates.Add(HabBot.State.Farm);
                break;
            case HabBotTrait.Type.Intelligence:
                possibleStates.Add(HabBot.State.Heal);
                possibleStates.Add(HabBot.State.Build);
                break;
        }
        return possibleStates[UnityEngine.Random.Range(0, possibleStates.Count)];
    }
    public HabBotTrait GetTrait(HabBotTrait.Type type)
    {
        foreach (HabBotTrait trait in _traits)
        {
            if (trait._type == type)
                return trait;
        }
        return null;
    }
    public void SetTrait(HabBotTrait.Type type, float val)
    {
        HabBotTrait trait = GetTrait(type);
        trait._val = val;
        OnTraitsChange?.Invoke(null, EventArgs.Empty);
    }
}
[Serializable]
public class HabBotTrait
{
    public int _index;
    public enum Type
    {
        Laziness,
        Strength,
        Speed,
        Intelligence,
        Building,
        Machining,
        Mining,
        Foraging,
        Farming,
        Healing,
    }
    public Type _type;
    [Range(0f, 1f)]
    public float _val;
    public bool _editable = false;

    public HabBotTrait(int index, Type type, float val, bool editable = false)
    {
        _index = index;
        _type = type;
        _val = val;
        _editable = editable;
    }
}
[Serializable]
public class Vitality
{
    public string _name;
    public int _val;
    public event EventHandler OnValueChange;

    public void Damage(int val)
    {
        _val -= val;
        if (_val < 0)
            _val = 0;

        OnValueChange?.Invoke(null, EventArgs.Empty);
    }
    public void Heal(int val)
    {
        _val += val;
        if (_val > 100)
            _val = 100;
        
        OnValueChange?.Invoke(null, EventArgs.Empty);
    }
    public float GetVitalityNormalized()
    {
        return (float)(_val/100);
    }
}

[Serializable]
public class HabBotAddon
{
    public enum Type
    {
        Sword,
        Axe,
        Drill,
        FarmTool,
        Welder,
    }
    public Type _type;
}