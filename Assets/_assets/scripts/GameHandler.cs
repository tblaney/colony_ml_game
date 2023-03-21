using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    public ResourceProcessor _resourceProcessor;
    public HabitationProcessor _habitationProcessor;


    void Awake()
    {
        Instance = this;
        Initialize();
        foreach (IHandler handler in transform.parent.GetComponentsInChildren<IHandler>())
        {
            handler.Initialize();
        }
    }
    void Initialize()
    {
        _resourceProcessor.Initialize();
        _habitationProcessor.Initialize();
        SaveSystem.Initialize();
    }
    void Start()
    {
        Load(0);
    }
    public void Save()
    {
        List<NodeSave> nodeSaves = _resourceProcessor.GetNodeSaves();
        Habitation habitation = _habitationProcessor._habitation;
        SaveData data = new SaveData()
        {
            _habitation = habitation,
            _nodes = nodeSaves,
        };
        SaveSystem.Save(data);
    }
    public void Load(int saveIndex = 0)
    {
        if (saveIndex == 0)
        {
            SaveSystem.SetIndex(SaveSystem.GetOpenIndex());
            _resourceProcessor.Load(null);
            _habitationProcessor.Load(null);
        } else
        {
            SaveData data = SaveSystem.Load(saveIndex);
            _habitationProcessor.Load(data._habitation);
            _resourceProcessor.Load(data._nodes);
        }
    }
}
