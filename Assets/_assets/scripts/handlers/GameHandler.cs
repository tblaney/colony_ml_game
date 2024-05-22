using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    public static bool _paused = false;
    public bool _loadOnStart = true;

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
        if (_loadOnStart)
            Load(0);
        else
            UIHandler.Instance.MenuStart();
    }
    public void Save()
    {
        List<Node> nodeSaves = HabitationHandler.Instance.GetAllNodes();
        Habitation habitation = HabitationHandler.Instance.GetHabitation();
        SaveData data = new SaveData()
        {
            _habitation = habitation,
            _nodes = nodeSaves,
            _inventories = ItemHandler.Instance._inventories,
            _threatSystem = ThreatHandler.Instance._threatSystem,
        };
        SaveSystem.Save(data);
    }
    public void Load(int saveIndex = 0)
    {
        if (saveIndex == 0)
        {
            SaveSystem.SetIndex(1);
            ItemHandler.Instance.Load(null);
            HabitationHandler.Instance.Load();
            ThreatHandler.Instance.Load(null);
        } else
        {
            SaveData data = SaveSystem.Load(saveIndex);
            ItemHandler.Instance.Load(data._inventories);
            HabitationHandler.Instance.Load(data._habitation, data._nodes);
            ThreatHandler.Instance.Load(data._threatSystem);
        }
    }
    public void Pause()
    {
        _paused = true;
        TimeHandler.Instance.SetTimeScaleLock(0f);
    }
    public void Resume()
    {
        _paused = false;
        TimeHandler.Instance.Lock(false);
    }
}