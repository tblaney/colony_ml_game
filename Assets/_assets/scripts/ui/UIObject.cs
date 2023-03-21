using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIObject : MonoBehaviour
{
    protected UIController _controller;

    void Awake()
    {
        _controller = GetComponent<UIController>();
    }

    public abstract void Initialize();
    public virtual void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
