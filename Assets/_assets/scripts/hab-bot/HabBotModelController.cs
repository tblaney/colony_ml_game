using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using System;

public class HabBotModelController : MonoBehaviour
{
    public List<HabBotModel> _models;

    public void Activate(bool active = true, int modelIndex = 0)
    {
        if (!active)
        {

        } else
        {

        }
    }
    public HabBotModel GetModel(int index)
    {
        foreach (HabBotModel model in _models)
        {
            if (model._index == index)
                return model;
        }
        return null;
    }
    public HabBotModel GetModel(string name)
    {
        foreach (HabBotModel model in _models)
        {
            if (model._name == name)
                return model;
        }
        return null;
    }
}

[Serializable]
public class HabBotModel
{
    public string _name;
    public int _index;
    public HabBotAgent _agent;
}