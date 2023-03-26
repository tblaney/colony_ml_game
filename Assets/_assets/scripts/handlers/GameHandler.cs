using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;


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
        SaveSystem.Initialize();
    }
    void Start()
    {
        Load(0);
    }
    public void Save()
    {
        List<Node> nodeSaves = HabitationHandler.Instance.GetNodes();
        Habitation habitation = HabitationHandler.Instance.GetHabitation();
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
            HabitationHandler.Instance.Load();
        } else
        {
            SaveData data = SaveSystem.Load(saveIndex);
            HabitationHandler.Instance.Load(data._habitation, data._nodes);
        }
    }
}
