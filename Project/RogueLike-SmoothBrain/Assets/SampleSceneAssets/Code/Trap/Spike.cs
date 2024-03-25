using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] EventReference spikesUpSFX;
    [SerializeField] EventReference spikesDownSFX;
    private float startPosY;
    private float endPosY;
    private bool isOut;
    [SerializeField] GameObject spikesToMove;
    private int damage;
    List<IDamageable> entitiesToDealDamage;

    private void Awake()
    {
        startPosY = spikesToMove.transform.position.y;
        endPosY = spikesToMove.transform.position.y + 1.5f;
        entitiesToDealDamage = new List<IDamageable>();
        damage = 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (!isOut)
            {
                entitiesToDealDamage.Add(other.gameObject.GetComponent<IDamageable>());
                StartCoroutine(Active());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            if (isOut)
            {
               entitiesToDealDamage.Remove(other.gameObject.GetComponent<IDamageable>());
               StartCoroutine(Disable());
            }
        }
    }

    IEnumerator Active()
    {
        yield return new WaitForSeconds(0.3f);
        isOut = true;
        AudioManager.Instance.PlaySound(spikesUpSFX);
        while (spikesToMove.transform.position.y != endPosY)
        {
            spikesToMove.transform.position += Vector3.up / 20;
            if (spikesToMove.transform.position.y > endPosY)
            {
                spikesToMove.transform.position = new Vector3(spikesToMove.transform.position.x, endPosY, spikesToMove.transform.position.z);
            }
            yield return new WaitForSeconds(0.003f);
        }

        entitiesToDealDamage.ForEach(actualEntity => { actualEntity.ApplyDamage(damage); });
        

        StartCoroutine(WaitUntil());
    }

    IEnumerator WaitUntil()
    {
        yield return new WaitForSeconds(4f);
        if (isOut)
        {
            StartCoroutine(Disable());
        }
    }

    IEnumerator Disable()
    {
        AudioManager.Instance.PlaySound(spikesDownSFX);
        yield return new WaitForSeconds(0.3f);
        while (spikesToMove.transform.position.y != startPosY)
        {
            spikesToMove.transform.position -= Vector3.up / 20;
            if (spikesToMove.transform.position.y < startPosY)
            {
                spikesToMove.transform.position = new Vector3(spikesToMove.transform.position.x, startPosY, spikesToMove.transform.position.z);
            }
            yield return new WaitForSeconds(0.003f);
        }
        isOut = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
