using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeHandler : MonoBehaviour
{
    public static TimeHandler Instance;

    public static event EventHandler OnMidnight;

    public void Initialize()
    {
        Instance = this;
    }


}
