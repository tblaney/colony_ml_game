using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy 
{
    Vitality GetVitality(); // get vitality
    string[] GetStrings(); // name, description
    Vector3 GetPosition();
    ITarget GetTarget();
}
