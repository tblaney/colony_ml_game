using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabBotLightController : MonoBehaviour
{
    Light _light;

    void Awake()
    {
        _light = GetComponent<Light>();
    }
}
