using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public InteractableEnemy _interactable;
    NavigationController _nav;
    AnimatorHandler _animator;

    void Awake()
    {
        _nav = GetComponent<NavigationController>();
        _animator = GetComponent<AnimatorHandler>();
    }
    void Start()
    {
        _interactable.Setup(GetComponent<IEnemy>());
    }
    void Update()
    {
        float magnitude = _nav.GetVelocity().magnitude;
        if (magnitude < 0.2f)
            magnitude = 0f;
        _animator.SetFloat("Speed", magnitude);
    }
}
