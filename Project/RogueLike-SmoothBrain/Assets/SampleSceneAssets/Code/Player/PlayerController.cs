using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Transform cameraTransform;
    Vector2 direction = Vector2.zero;
    CharacterController characterController;
    readonly float speed = 6f;
    readonly float smoothTime = 0.05f;
    float currentVelocity = 0f;
    float currentTargetAngle = 0f;

    public Vector2 Direction
    {
        get { return direction; }
    }
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, currentTargetAngle, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        if (direction.x != 0f || direction.y != 0f)
        {
            currentTargetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + cameraTransform.rotation.eulerAngles.y;
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            characterController.Move(speed * Time.deltaTime * (camForward  * direction.y + camRight * direction.x).normalized);
        }
    }

    public void ReadDirection(InputAction.CallbackContext ctx)
    {
        direction = ctx.ReadValue<Vector2>();
    }
    
}
