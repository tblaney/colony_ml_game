using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    public enum State
    {
        Manual,
        Follow,
    }
    public State _state;
    public float _movementSpeed = 12f;
    ITarget _target;


    void Update()
    {
        Vector3 targetPosition = transform.position;
        float movementSpeed = _movementSpeed;
        float sprintFactor = 1f;
        switch (_state)
        {
            case State.Manual:
                float x = Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");
                Vector3 moveDirection = (new Vector3(transform.forward.x, 0f, transform.forward.z))*y + transform.right*x;
                moveDirection = moveDirection.normalized;
                targetPosition = transform.position + moveDirection;
                break;
            case State.Follow:
                targetPosition = _target.GetPosition();
                sprintFactor = 0.6f;
                movementSpeed = 10f;
                break;
        }
        if (Input.GetKey(KeyCode.LeftShift))
            sprintFactor = 1.8f;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime*movementSpeed*sprintFactor);
    }
    public void SetTarget(ITarget followTarget)
    {
        _target = followTarget;
        _state = State.Follow;
    }
    public void SetManual()
    {
        _target = null;
        _state = State.Manual;
    }
}
