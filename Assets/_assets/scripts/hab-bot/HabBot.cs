using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HabBot : ITarget
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
        Craft,
        Stockpile,
        Recreation,
        Machine,
        Haul,
        Destroy,
    }
    public State _state;
    public State _stateDefault;
    public List<Vitality> _vitalities;
    public HabBotTraits _traits;
    public List<HabBotAddon> _addons;
    public int _inventoryIndex;
    public bool _stateLock;
    public bool _stateCooldown;
    public Vector3 _position;
    public Vector3 _velocity;
    float _timer;
    // events
    public event EventHandler<StateChangeEventArgs> OnStateChange;
    public event EventHandler<StateChangeEventArgs> OnStateAccessChange;
    public event EventHandler OnColorChange;
    public event EventHandler OnDeath;
    public class StateChangeEventArgs : EventArgs
    {
        public HabBot _bot;
    }

    public void InitializeRandom(int index)
    {
        _name = "00" + index.ToString();
        _index = index;
        _addons = new List<HabBotAddon>();
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
                case 3:
                    vitality._name = "happiness";
                    vitality._val = 50;
                    break;
            }
            _vitalities.Add(vitality);
        }
        _addons.Add(new HabBotAddon(){_type = HabBotAddon.Type.Drill});
        _addons.Add(new HabBotAddon(){_type = HabBotAddon.Type.Axe});
        _addons.Add(new HabBotAddon(){_type = HabBotAddon.Type.Sword});
    }
    public void Initialize()
    {
        if (_inventoryIndex == 0)
        {
            _inventoryIndex = ItemHandler.Instance.NewInventory();
            ItemInventory inventory = ItemHandler.Instance.GetItemInventory(_inventoryIndex);
            //inventory.AddItem(2, 1);
            //inventory.AddItem(3, 1);
            //inventory.AddItem(4, 1);
            //inventory.AddItem(5, 1);
            //inventory.AddItem(6, 1);
        }

        //HabitationHandler.Instance.AddInventory(_inventoryIndex);
        _stateCooldown = false;
        _stateLock = false;
    }
    public void UpdateBot()
    {
        VitalityUpdate();
    }
    void VitalityUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer >= 1f)
        {
            Vitality energy = GetVitality("energy");
            Vitality hunger = GetVitality("hunger");
            energy.Damage(1);
            hunger.Damage(1);
            _timer = 0f;
        }
    }
    public void DetermineDefaultState()
    {
        _stateDefault = _traits.GetDefaultState();
    }
    public void SetState(State state)
    {
        // everything related to bot switching states is executed from this function (state change event)
        if (_stateCooldown | _stateLock)
            return;
        _state = state;
        StateChangeEventArgs eventArgs = new StateChangeEventArgs() {_bot = this};
        OnStateChange?.Invoke(null, eventArgs);
        Action OnDelayFunc = () =>
        {
            _stateCooldown = false;
            OnStateAccessChange?.Invoke(null, eventArgs);
        };
        ActionHandler.Instance.ActionOnDelayUnscaled(5f, OnDelayFunc);
        _stateCooldown = true;
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
    public void SetColor(Color color)
    {
        _traits._color = color;
        OnColorChange?.Invoke(null, EventArgs.Empty);
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
    public Vector3 GetPosition()
    {
        return _position;
    }
    public bool Damage(int val)
    {
        Vitality vitality = GetVitality("health");
        vitality.Damage(val);
        if (vitality._val <= 0)
        {
            return false;
        }
        return true;
    }
    public bool ContainsAddon(HabBotAddon.Type type)
    {
        if (_addons == null)
            return false;
        foreach (HabBotAddon addon in _addons)
        {
            if (addon._type == type)
                return true;
        }
        return false;
    }
    public int GetDamage(State state)
    {
        switch (state)
        {
            case State.CollectMinerals:
                return (int)(1+(20f * _traits.GetTraitVal(HabBotTrait.Type.Strength)* _traits.GetTraitVal(HabBotTrait.Type.Mining)));
            case State.CollectTrees:
                return (int)(20f * _traits.GetTraitVal(HabBotTrait.Type.Strength)* _traits.GetTraitVal(HabBotTrait.Type.Foraging));
        }
        return 0;
    }
    public float GetInteractTime(State state)
    {
        switch (state)
        {
            case State.Build:
                return (30f*(1+_traits.GetTraitVal(HabBotTrait.Type.Laziness)))*(1.1f-_traits.GetTraitVal(HabBotTrait.Type.Intelligence))*(1.1f-_traits.GetTraitVal(HabBotTrait.Type.Building));
            case State.Farm:
                return (30f*(1+_traits.GetTraitVal(HabBotTrait.Type.Laziness)))*(1.1f-_traits.GetTraitVal(HabBotTrait.Type.Intelligence))*(1.1f-_traits.GetTraitVal(HabBotTrait.Type.Farming));
            case State.Machine:
                return (30f*(1+_traits.GetTraitVal(HabBotTrait.Type.Laziness)))*(1.1f-_traits.GetTraitVal(HabBotTrait.Type.Intelligence))*(1.1f-_traits.GetTraitVal(HabBotTrait.Type.Machining));
            case State.CollectMinerals:
                return (5f*(1.1f-_traits.GetTraitVal(HabBotTrait.Type.Strength)));
        }
        return 30f;
    }
}

[Serializable]
public class HabBotTraits
{
    public List<HabBotTrait> _traits;
    public Color _color;
    public event EventHandler OnTraitsChange;
    public void InitializeRandom()
    {
        _traits = new List<HabBotTrait>();
        int i = 0;
        foreach(HabBotTrait.Type traitType in Enum.GetValues(typeof(HabBotTrait.Type)))
        {
            float val = UnityEngine.Random.Range(0.2f, 0.9f);
            if (traitType == HabBotTrait.Type.Speed)
                val = UnityEngine.Random.Range(0.6f, 0.95f);
            bool editable = false;
            if (i >= 4)
            {
                editable = true;
            }
            HabBotTrait trait = new HabBotTrait(i, traitType, val, editable);
            _traits.Add(trait);
            i++;
        }
        _color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
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
                possibleStates.Add(HabBot.State.Machine);
                possibleStates.Add(HabBot.State.Rescue);
                possibleStates.Add(HabBot.State.Farm);
                break;
            case HabBotTrait.Type.Intelligence:
                possibleStates.Add(HabBot.State.Machine);
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
    public float GetTraitVal(HabBotTrait.Type type)
    {
        HabBotTrait trait = GetTrait(type);
        return trait._val;
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
        Crafting,
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
        return (float)((float)_val/100f);
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
