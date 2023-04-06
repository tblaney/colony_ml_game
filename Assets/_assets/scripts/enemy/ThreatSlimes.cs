using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreatSlimes : Threat
{
    public override void Initialize()
    {

    }
    public override void Spawn(Bounds bounds, int amount = 0)
    {
        int amountTotal = amount;
        if (amountTotal == 0)
            amountTotal = UnityEngine.Random.Range(_amountRange.x, _amountRange.y);
            
        for (int i = 0; i < amountTotal; i++)
        {
            Vector3 position = GetOpenPositionFunc(bounds);
            GameObject obj = Instantiate(_prefab, position, Quaternion.identity, this.transform);
            EnemyAgent agent = obj.GetComponent<EnemyAgent>(); 
            agent.Setup(DestroyEnemyCallback);
            _enemies.Add(agent);
        }
    }
    void DestroyEnemyCallback(EnemyAgent agent)
    {
        if (_enemies.Contains(agent))
            _enemies.Remove(agent);
        
        if (_enemies.Count == 0)
            Activate(false);
        
        _amountActive = _enemies.Count;
    }
}
