using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Image>().material.renderQueue = 4000;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
