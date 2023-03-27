using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private MaterialController _controller;

    public virtual void InteractHover(bool isIn)
    {
        float val = 1f;
        if (!isIn)
            val = 0f;

        if (_controller != null)
            _controller.SetFloat(val, 1);
    }
    public abstract void Interact();
}
