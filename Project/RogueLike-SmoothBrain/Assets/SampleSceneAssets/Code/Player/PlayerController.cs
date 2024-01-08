using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    Vector2 direction = Vector2.zero;
    CharacterController characterController;
    float speed = 2f;
    float smoothTime = 0.05f;
    float currentVelocity;
    float currentTargetAngle = 0;

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
        transform.rotation = Quaternion.Euler(0, angle, 0);

        if (direction.x != 0 || direction.y != 0)
        {
            currentTargetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + 45;
            characterController.Move(new Vector3(direction.x * Time.deltaTime * speed, 0, direction.y * Time.deltaTime * speed));
        }
    }

    public void ReadDirection(InputAction.CallbackContext ctx)
    {
        direction = ctx.ReadValue<Vector2>();
    }
  
}
