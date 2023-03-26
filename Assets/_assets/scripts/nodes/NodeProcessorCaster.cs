using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeProcessorCaster : MonoBehaviour
{
    public LayerMask _maskBlocker;

    public RaycastHit[] BlockerCast(Vector3 position)
    {
        position.y = 100f;
        RaycastHit[] objs = Physics.RaycastAll(position, Vector3.down, 200f, _maskBlocker);
        return objs;
    }
}
