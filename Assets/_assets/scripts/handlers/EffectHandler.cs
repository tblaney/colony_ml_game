using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EffectHandler : MonoBehaviour, IHandler
{
    public static EffectHandler Instance;

    [Header("Inputs:")]
    [SerializeField] private List<Effect> _effects;

    List<EffectModifier> _runing_effects;
    public int _runtime_identifier_count;

    public void Initialize()
    {
        Instance = this;

        _runing_effects = new List<EffectModifier>();
    }

    public int GetRuntimeIdentifier()
    {
        _runtime_identifier_count++;
        
        return _runtime_identifier_count;
    }

    public int SpawnEffect(int index, Vector3 position, out EffectModifier modifier, Transform parent = null)
    {
        if (parent == null)
            parent = this.transform;

        Effect effect = GetEffect(index);

        Transform effectTransform = Instantiate(effect._prefab, parent);
        effectTransform.position = position;

        modifier = effectTransform.GetComponent<EffectModifier>();

        int runtime_identifier = GetRuntimeIdentifier();

        if (effect._looping)
        {
            _runing_effects.Add(modifier);
            modifier.RunLoop(runtime_identifier);
        } else
        {
            modifier.Run(effect._time);
        }

        return runtime_identifier;
    }
    public int SpawnEffect(int index, Vector3 position, Transform parent = null)
    {
        if (parent == null)
            parent = this.transform;

        Effect effect = GetEffect(index);

        Transform effectTransform = Instantiate(effect._prefab, parent);
        effectTransform.position = position;

        EffectModifier modifier = effectTransform.GetComponent<EffectModifier>();

        int runtime_identifier = GetRuntimeIdentifier();

        if (effect._looping)
        {
            _runing_effects.Add(modifier);
            modifier.RunLoop(runtime_identifier);
        } else
        {
            modifier.Run(effect._time);
        }

        return runtime_identifier;
    }

    public void StopEffect(int index)
    {
        List<EffectModifier> removal_list = new List<EffectModifier>();
        foreach (EffectModifier modifier in _runing_effects)
        {
            if (modifier._index == index)
            {
                modifier.Stop();
                removal_list.Add(modifier);
            }
        }

        foreach (EffectModifier modifier in removal_list)
        {
            _runing_effects.Remove(modifier);
        }
    }

    public EffectModifier GetRuntimeModifier(int index)
    {
        foreach (EffectModifier modifier in _runing_effects)
        {
            if (modifier._index == index)
            {
                return modifier;
            }
        }

        return null;
    }

    Effect GetEffect(int index)
    {
        foreach (Effect effect in _effects)
        {
            if (effect._index == index)
            {
                return effect;
            }
        }

        return null;
    }



}

[Serializable]
public class Effect
{
    public string _name;
    public int _index;
    public float _time;

    public bool _looping;

    public Transform _prefab;

}
