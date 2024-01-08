using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update

    Vector2 direction = Vector2.zero;
    NavMeshAgent agent;
    public Vector2 Direction
    {
        get { return direction; }
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.hasPath)
        {

        }
        agent.SetDestination(this.transform.position + new Vector3(direction.x, 0, direction.y));
    }

    public void ReadDirection(InputAction.CallbackContext ctx)
    {
        direction = ctx.ReadValue<Vector2>();
    }
    
}
