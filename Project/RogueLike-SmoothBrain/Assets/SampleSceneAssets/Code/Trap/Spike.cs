using System.Collections;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private Coroutine spikeActivate;
    private Coroutine spikeDisable;
    private Coroutine spikeWaitForDisable;
    private float startPosY;
    private float endPosY;
    private bool isOut;
    [SerializeField] private GameObject spikesToMove;

    private void Awake()
    {
        startPosY = spikesToMove.transform.position.y;
        endPosY = spikesToMove.transform.position.y + 20;
        spikeActivate = null;
        spikeDisable = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (!isOut)
            {
                spikeActivate = StartCoroutine(Active());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if (isOut)
            {
                spikeDisable = StartCoroutine(Disable());
            }
        }
    }

    IEnumerator Active()
    {
        yield return new WaitForSeconds(0.3f);
        isOut = true;
        spikeActivate = null;
        spikeWaitForDisable = StartCoroutine(WaitUntil());
    }

    IEnumerator WaitUntil()
    {
        yield return new WaitForSeconds(4f);
        if (isOut)
        {
            spikeDisable = StartCoroutine(Disable());
        }
        spikeWaitForDisable = null;
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(0.3f); ;
        isOut = false;
        spikeDisable = null;
    }
}
