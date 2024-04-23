using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dlamerd : MonoBehaviour
{
    [SerializeField] float speed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, gameObject.transform.position + transform.forward, Time.deltaTime * speed);
    }
}
