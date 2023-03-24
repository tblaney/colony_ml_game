using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHandler : MonoBehaviour, IHandler
{
    public static UserHandler Instance;
    [SerializeField] private UserController _controller;

    public void Initialize()
    {
        Instance = this;
    }
    public void SetUserState(UserController.State state)
    {
        _controller.SetState(state);
    }
}
