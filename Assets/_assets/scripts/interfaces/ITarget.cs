using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget
{
    Vector3 GetPosition();
    bool Damage(int amount); // will return true if alive, false if dead
}
