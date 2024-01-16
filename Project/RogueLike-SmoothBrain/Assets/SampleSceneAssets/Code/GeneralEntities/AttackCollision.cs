using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollision : MonoBehaviour
{
    public enum CollisionType
    {
        Ray,
        Box,
        Capsule,
        Sphere
    }

    [SerializeField] private CollisionType collisionType;

    private void OnDrawGizmos()
    {
        //Physics.cast
    }

    private void OnValidate()
    {
        Initialize();
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        
    }

    public void Launch()
    {

    }


}
