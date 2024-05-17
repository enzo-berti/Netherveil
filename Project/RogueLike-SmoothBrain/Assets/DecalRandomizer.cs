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
        transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
    }
}
