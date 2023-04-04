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
    public bool _sprinting;
    public Vector2 _mousePosition;
    public bool _active = true;
    ITarget _target;
    Coroutine _update;

    void Start()
    {
        //_update = StartCoroutine(UpdateRoutine());
    }
    void Update()
    {
        UpdateCamera();
    }
    void UpdateCamera()
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
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    sprintFactor = 2.4f;
                    _sprinting = true;
                } else
                {
                    _sprinting = false;
                }
                break;
            case State.Follow:
                targetPosition = _target.GetPosition();
                sprintFactor = 0.6f;
                movementSpeed = 6f;
                break;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime*movementSpeed*sprintFactor);

        RotateUpdate();
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
    void RotateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 delta = mousePosition - _mousePosition;
            if (_mousePosition == default(Vector2))
                delta.x = 0f;
            
            transform.Rotate(0f, delta.x*Time.deltaTime*4f, 0f, Space.World);
            _mousePosition = mousePosition;
        } else
        {
            _mousePosition = default(Vector2);
        }
    }

    private IEnumerator UpdateRoutine()
    {
        while (_active)
        {
            UpdateCamera();
            yield return null;
        }
    }
}
