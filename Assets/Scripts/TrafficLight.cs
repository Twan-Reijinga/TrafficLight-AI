using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public bool isGreen = false;

    private BoxCollider bc;

    void Start(){
        bc = GetComponent<BoxCollider>();
    }

    void Update(){
        bc.enabled = !isGreen;
    }
}
