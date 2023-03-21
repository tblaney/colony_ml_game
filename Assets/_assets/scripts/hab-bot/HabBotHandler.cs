using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabBotHandler : MonoBehaviour
{
    public static HabBotHandler Instance;

    void Awake()
    {
        Instance = this;
    }
}

