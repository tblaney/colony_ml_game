using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIObject : MonoBehaviour
{
    protected UIController _controller;

    void Awake()
    {
        _controller = GetComponent<UIController>();
        Initialize();
    }
    public void ActivateUI(bool active = true)
    {
        _controller.ActivateBehaviour("activate", active);
        Activate(active);
    }
    public virtual void Activate(bool active = true)
    {

    }
    public abstract void Initialize();
    public virtual void DestroyUI()
    {
        Destroy(this.gameObject);
    }
}
