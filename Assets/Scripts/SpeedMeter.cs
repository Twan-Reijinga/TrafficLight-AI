using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedMeter : MonoBehaviour
{
    public Vector3 velocity = Vector3.zero;

    private Vector3 lastPos = Vector3.zero;

    void Start(){
        lastPos = transform.position;
    }

    void Update(){
        velocity = (transform.position - lastPos)/Time.deltaTime;
        lastPos = transform.position;
    }
}
