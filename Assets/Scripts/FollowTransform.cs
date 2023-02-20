using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;

    void Update()
    {
        if (_target == null)
            return;
        
        transform.position = _target.position + _offset;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }
}
