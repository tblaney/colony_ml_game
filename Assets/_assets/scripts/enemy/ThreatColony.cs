using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatColony : Threat
{
    ColonistArea _area;
    [SerializeField] private List<MeshRenderer> _colonySpawnAreas;
    public override void Initialize()
    {
        _area = GetComponent<ColonistArea>();
    }
    public override void Spawn(Bounds bounds, int amount)
    {
        int amountTotal = UnityEngine.Random.Range(_amountRange.x, _amountRange.y);
        if (amount != 0)
            amountTotal = amount;
        _area.Initialize((_colonySpawnAreas[UnityEngine.Random.Range(0, _colonySpawnAreas.Count)]), amountTotal, ColonistSpawnCallback, ColonistDeathCallback, ColonyDeathCallback);
    }
    void ColonistDeathCallback()
    {
        Refresh();
        _amountActive = _enemies.Count;
    }
    void ColonistSpawnCallback()
    {
        Refresh();
        _amountActive = _enemies.Count;
    }
    void ColonyDeathCallback()
    {
        Refresh();
        Activate(false);
    }
    void Refresh()
    {
        _enemies = new List<IEnemy>();
        foreach (ColonistAgent agent in _area.colonistAgents)
        {
            _enemies.Add(agent);
        }
    }
}
