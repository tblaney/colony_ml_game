using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BuiltNodeBehaviourVolleyball : BuiltNodeBehaviour
{
    public VolleyballEnvController _env;
    public List<Transform> _positions;
    List<VolleyballPlayer> _players;
    bool _playing = false;
    public override void StartBehaviour()
    {
        _players = new List<VolleyballPlayer>();
        _playing = false;
        _env.Initialize();
    }
    public override void UpdateBehaviour()
    {

    }   
    public Vector3 QueueAgent(VolleyballAgent agent, out Team team)
    {
        team = Team.Default;
        if (_players.Count == 0)
        {   
            team = Team.Purple;
            _players.Add(new VolleyballPlayer(agent){});
            return _positions[0].position;
        } else if (_players.Count == 1)
        {
            team = Team.Blue;
            _players.Add(new VolleyballPlayer(agent){});
            return _positions[1].position;
        } else
        {
            return default(Vector3);
        }
    }
    public void AgentReady(VolleyballAgent agent, Action AgentStartFunc, Action AgentStopFunc)
    {
        VolleyballPlayer player = GetPlayer(agent);
        player._ready = true;
        player.StartFunc = AgentStartFunc;
        player.StopFunc = AgentStopFunc;
        if (GetReadyAgents() == 2)
        {
            StartGame();
        }
    }
    public void UnreadyAgent(VolleyballAgent agent)
    {
        VolleyballPlayer player = GetPlayer(agent);
        if (player != null)
        {
            player._agent = null;
            _players.Remove(player);
            if (_playing)
            {
                StopGame();
            } 
        }
    }
    void StartGame()
    {
        foreach (VolleyballPlayer player in _players)
        {
            if (player.StartFunc != null)
                player.StartFunc();
        }
        _env.StartGame(_players[0]._agent, _players[1]._agent);
        _playing = true;
    }
    void StopGame()
    {
        foreach (VolleyballPlayer player in _players)
        {
            if (player.StopFunc != null)
                player.StopFunc();
        }
        _env.StopGame();
        _playing = false;
        _players.Clear();
    }
    public VolleyballPlayer GetPlayer(VolleyballAgent agent)
    {
        foreach (VolleyballPlayer player in _players)
        {
            if (player._agent == agent)
                return player;
        }
        return null;
    }
    int GetReadyAgents()
    {
        int i = 0;
        foreach (VolleyballPlayer player in _players)
        {
            if (player._ready)
                i++;
        }
        return i;
    }
}

public class VolleyballPlayer
{
    public VolleyballAgent _agent;
    public bool _ready;
    public Action StartFunc;
    public Action StopFunc;

    public VolleyballPlayer(VolleyballAgent agent)
    {
        _agent = agent;
    }
}