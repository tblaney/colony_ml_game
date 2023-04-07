using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Interactable : MonoBehaviour
{
    [Header("Renderer Inputs:")]
    public List<MeshRenderer> _renderers;
    [Header("Material Inputs:")]
    public List<MaterialController> _controllers;
    public Action<bool> InteractHoverFunc;
    List<GameObject> _objs;

    void Awake()
    {
        _objs = new List<GameObject>();
    }
    public virtual void InteractHover(bool isIn)
    {
        Debug.Log("Interact Hover: " + this.gameObject.name + ", " + isIn);

        float val = 0f;
        if (isIn)
            val = 1f;
        foreach (MaterialController controller in _controllers)
        {
            controller.SetFloat(val, 1);
        }
        if (isIn)
        {   
            Clear();
            foreach (MeshRenderer mesh in _renderers)
            {
                _objs.Add(HighlightHandler.Instance.NewHighlightObject(mesh));
            }
        } else
        {
            Clear();
        }
    }
    void Clear()
    {
        foreach (GameObject obj in _objs)
        {
            Destroy(obj);
        }
        _objs.Clear();
    }
    public abstract void Interact();
}
