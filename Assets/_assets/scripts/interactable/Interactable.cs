using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private List<MaterialController> _controllers;
    public Action<bool> InteractHoverFunc;
    
    public virtual void InteractHover(bool isIn)
    {
        float val = 1f;
        if (!isIn)
            val = 0f;

        if (_controllers != null)
        {
            foreach (MaterialController controller in _controllers)
            {
                controller.SetFloat(val, 1);
            }
        }
    }
    public abstract void Interact();
}
