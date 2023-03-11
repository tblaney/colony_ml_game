using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonyHandler : MonoBehaviour
{
    public static ColonyHandler Instance;

    void Awake()
    {
        Instance = this;
    }
}
