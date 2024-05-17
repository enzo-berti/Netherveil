using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalRandomizer : MonoBehaviour
{
    [SerializeField] Material[] materials;
    DecalProjector decalProjector;

    void Awake()
    {
        decalProjector = GetComponent<DecalProjector>();
        decalProjector.material = materials[UnityEngine.Random.Range(0, materials.Length)];
        transform.rotation = Quaternion.Euler(gameObject.transform.rotation.eulerAngles.x, UnityEngine.Random.Range(gameObject.transform.rotation.eulerAngles.x, 360f), gameObject.transform.rotation.eulerAngles.z);
    }
}
