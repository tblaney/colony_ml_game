using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThreatHandler : MonoBehaviour, IHandler
{
    public static ThreatHandler Instance;
    [Header("Input:")]
    public NodeProcessor _processor;
    public ThreatSystem _threatSystem;
    public int _refreshRateMinutes = 1;
    public List<MeshRenderer> _spawnBounds;
    [Header("Test:")]
    public bool _testExecute;

    float _timer;
    bool _active;

    public void Initialize()
    {
        Instance = this;
        Setup();
        _active = false;
    }
    void Setup()
    {
        foreach (Threat threat in _threatSystem._threats)
        {
            threat.GetOpenPositionFunc = _processor.GetOpenPositionBounds;
        }
    }
    void Update()
    {
        if (_testExecute)
        {
            TrySpawnThreat();
            _testExecute = false;
        }
        if (!_active)
            return;
        _timer += Time.deltaTime;
        if (_timer > _refreshRateMinutes*60f)
        {
            TrySpawnThreat();
            _timer = 0f;
        }
    }
    public void Load(ThreatSystem threatSystem)
    {
        if (threatSystem != null)
        {
            _threatSystem = threatSystem;
            // need to check for active threats and spawn
            foreach (Threat threat in _threatSystem._threats)
            {
                if (threat._active)
                {
                    SpawnThreat(threat);
                    break;
                }
            }
        }
        _active = true;
        TimeHandler._timeWorld.OnMidnight += Time_OnMidnight;
        _threatSystem.RefreshWealth();
    }
    void TrySpawnThreat()
    {
        if (IsThreatActive())
            return;
        // get list of threats we can spawn
        List<Threat> availableThreats = GetAvailableThreats();
        if (availableThreats.Count > 0)
        {
            Threat threat = availableThreats[UnityEngine.Random.Range(0, availableThreats.Count)];
            SpawnThreat(threat);
        }
    }
    public void SpawnThreat(Threat threat)
    {
        threat.SpawnThreat(_spawnBounds[UnityEngine.Random.Range(0, _spawnBounds.Count)].bounds);
    }
    public Threat GetThreat(int index)
    {
        foreach (Threat threat in _threatSystem._threats)
        {
            if (threat._index == index)
                return threat;
        }
        return null;
    }
    public bool IsThreatActive()
    {
        foreach (Threat threat in _threatSystem._threats)
        {
            if (threat._active)
                return true;
        }
        return false;
    }
    void Time_OnMidnight(object sender, EventArgs e)
    {
        _threatSystem.RefreshWealth();
    }
    List<Threat> GetAvailableThreats()
    {
        List<Threat> threats = new List<Threat>();
        foreach (Threat threat in _threatSystem._threats)
        {
            if (threat._cost < _threatSystem._wealth)
            {
                threats.Add(threat);
            }
        }
        return threats;
    }
}
[Serializable]
public class ThreatSystem
{
    // class to save if we want to
    public List<Threat> _threats;
    public int _wealth;

    public void RefreshWealth()
    {
        // wealth points refresh
        // gets called at midnight every day
        int amountBots = HabitationHandler.Instance.GetBotAmount();
        int amountItems = HabitationHandler.Instance.GetItemAmount();

        _wealth += amountBots*20 + amountItems;
    }
}
[Serializable]
public abstract class Threat : MonoBehaviour, ITarget
{
    [Header("Input:")]
    public string _name;
    public int _index;
    public int _cost = 50;
    public GameObject _prefab;
    public Vector2Int _amountRange;
    public string _notificationText;

    [Header("Debug:")]
    public bool _active;

    // cache:
    public Func<Bounds, Vector3Int> GetOpenPositionFunc;
    protected int _amountActive;
    Bounds _bounds;
    Notification _notification;
    protected List<IEnemy> _enemies;

    void Awake()
    {
        _enemies = new List<IEnemy>();
        Initialize();
    }
    public virtual void Activate(bool val)
    {
        this._active = val;
    }
    public virtual void Initialize(){}

    public void SpawnThreat(Bounds bounds, int amount = 0)
    {
        _bounds = bounds;
        Spawn(bounds, amount);
        _amountActive = _enemies.Count;
        SetupNotification();
        Activate(true);
    }
    void SetupNotification()
    {
        Notification notification = NotificationHandler.Instance.NewNotification(Notification.Type.General, _notificationText, NotificationClickCallback);
        _notification = notification;
    }
    void NotificationClickCallback()
    {
        UserHandler._target = this;
        UserHandler.Instance.SetUserState(UserController.State.Move);
        NotificationHandler.Instance.ClearNotification(_notification);
        _notification = null;
    }
    public abstract void Spawn(Bounds bounds, int amount = 0);
    public Vector3 GetPosition()
    {
        return _bounds.center;
    }
}
