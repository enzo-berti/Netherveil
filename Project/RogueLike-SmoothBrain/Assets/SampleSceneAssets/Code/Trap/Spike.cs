using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] EventReference spikesUpSFX;
    [SerializeField] EventReference spikesDownSFX;
    private FMOD.Studio.EventInstance spikesUpEvent;
    private FMOD.Studio.EventInstance spikesDownEvent;
    private float startPosY;
    private float endPosY;
    private bool isOut;
    [SerializeField] GameObject spikesToMove;
    private int damage;
    private float waitUntilTimer;
    List<IDamageable> entitiesToDealDamage = new List<IDamageable>();
    private float timerCheckEntitiesList;

    private void Awake()
    {
        startPosY = spikesToMove.transform.position.y;
        endPosY = spikesToMove.transform.position.y + Mathf.Abs(spikesToMove.transform.localPosition.y);
        waitUntilTimer = 3f;
        damage = 10;
        isOut = false;

        spikesUpEvent = RuntimeManager.CreateInstance(spikesUpSFX);
        spikesUpEvent.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        spikesDownEvent = RuntimeManager.CreateInstance(spikesDownSFX);
        spikesDownEvent.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<IDamageable>(out var damageable) && (damageable as MonoBehaviour).GetComponent<Entity>().canTriggerTraps)
        {
            entitiesToDealDamage.Add(damageable);
            if (!isOut)
            {
                StartCoroutine(Active());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (timerCheckEntitiesList >= 2f)
        {
            timerCheckEntitiesList = 0f;

            List<IDamageable> entitiesToKeep = new List<IDamageable>();

            if(entitiesToDealDamage != null && entitiesToDealDamage.Count > 0)
            {
                foreach (IDamageable entity in entitiesToDealDamage)
                {
                    IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
                    if (damageable != null && entity == damageable)
                    {
                        entitiesToKeep.Add(entity);
                    }
                }
            }

            entitiesToDealDamage = entitiesToKeep;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            entitiesToDealDamage.Remove(damageable);

            waitUntilTimer = 2f;
            if (isOut && !entitiesToDealDamage.Any())
            {
                StartCoroutine(Disable());
            }
            else
            {
                StopCoroutine(Active());
                isOut = false;
                StartCoroutine(WaitUntil());
            }
        }
    }

    private void Update()
    {
        if(entitiesToDealDamage != null && entitiesToDealDamage.Count > 0)
        {
            entitiesToDealDamage.RemoveAll(x => (x as MonoBehaviour) == null);
        }
        timerCheckEntitiesList += Time.deltaTime;
    }

    IEnumerator Active()
    {
        yield return new WaitForSeconds(0.15f);
        AudioManager.Instance.StopSound(spikesDownEvent, FMOD.Studio.STOP_MODE.Immediate);
        AudioManager.Instance.PlaySound(spikesUpSFX, transform.position);
        while (spikesToMove.transform.position.y != endPosY)
        {
            spikesToMove.transform.position += Vector3.up * Time.deltaTime * 10;
            if (spikesToMove.transform.position.y > endPosY)
            {
                spikesToMove.transform.position = new Vector3(spikesToMove.transform.position.x, endPosY, spikesToMove.transform.position.z);
            }
            yield return new WaitForSeconds(0.003f);
        }
        isOut = true;
        entitiesToDealDamage.ForEach(actualEntity => { actualEntity.ApplyDamage(damage, null); });

        StartCoroutine(WaitUntil());
    }

    IEnumerator WaitUntil()
    {
        yield return new WaitForSeconds(waitUntilTimer);
        if (isOut)
        {
            StartCoroutine(Disable());
        }
    }

    IEnumerator Disable()
    {

        yield return new WaitForSeconds(0.15f);
        AudioManager.Instance.StopSound(spikesUpEvent, FMOD.Studio.STOP_MODE.Immediate);
        AudioManager.Instance.PlaySound(spikesDownSFX, transform.position);
        while (spikesToMove.transform.position.y != startPosY)
        {
            spikesToMove.transform.position -= Vector3.up * Time.deltaTime * 15;
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
