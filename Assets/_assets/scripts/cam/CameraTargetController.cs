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
    public State state;
    public float movementSpeed = 12f;
    ITarget target;


    void Update()
    {
        Vector3 targetPosition = transform.position;
        float sprintFactor = 1f;
        switch (state)
        {
            case State.Manual:
                float x = Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");
                Vector3 moveDirection = (new Vector3(transform.forward.x, 0f, transform.forward.z))*y + transform.right*x;
                moveDirection = moveDirection.normalized;
                targetPosition = transform.position + moveDirection;
                break;
            case State.Follow:
                targetPosition = target.GetPosition();
                sprintFactor = 1.8f;
                break;
        }
        if (Input.GetKey(KeyCode.LeftShift))
            sprintFactor = 1.8f;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.unscaledDeltaTime*movementSpeed*sprintFactor);
    }
    public void SetTarget(ITarget followTarget)
    {
        target = followTarget;
        state = State.Follow;
    }
    public void SetManual()
    {
        target = null;
        state = State.Manual;
    }
}
