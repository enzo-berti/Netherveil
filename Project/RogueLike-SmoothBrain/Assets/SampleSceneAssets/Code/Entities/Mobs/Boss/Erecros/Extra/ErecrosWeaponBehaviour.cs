using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErecrosWeaponBehaviour : MonoBehaviour
{
    Rigidbody rb;

    [HideInInspector] public bool hitMap = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        Reset();
    }

    public void Reset()
    {
        hitMap = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.velocity = Vector3.zero;

        hitMap = true;
    }
}