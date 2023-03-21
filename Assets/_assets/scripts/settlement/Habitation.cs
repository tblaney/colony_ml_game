using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Habitation
{
    // main class that stores all information regarding the habitation, should just be able to load this in from a save
    public List<HabBot> _bots;
    public List<HabitationZone> _zones;

    public void NewHabitation()
    {
        _bots = new List<HabBot>();
        for (int i = 0; i < HabitationProcessor._parameters._botAmountStart; i++)
        {
            HabBot bot = new HabBot();
            bot.InitializeRandom(i);
            _bots.Add(bot);
        }
    }
    public Vector3 GetHabitationZonePosition(HabitationZone.Type zoneType)
    {
        HabitationZone zone = GetZone(zoneType);
        if (zone != null)
        {
            return zone.GetRandomPosition();
        }
        return default(Vector3);
    }
    public HabitationZone GetZone(HabitationZone.Type zoneType)
    {
        foreach (HabitationZone zone in _zones)
        {
            if (zone._type == zoneType)
                return zone;
        }
        return null;
    }
}
