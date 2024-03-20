using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    private CharacterController controller;
    Animator animator;
    private float playerSpeed = 10.0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);
        animator.SetFloat("Speed", move.magnitude, 0.1f, Time.deltaTime);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }
    }
}
