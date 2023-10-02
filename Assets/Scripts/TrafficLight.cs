using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public bool isGreen = false;
    public Material greenMat;
    public Material redMat;

    private BoxCollider bc;
    private MeshRenderer mr;

    void Start(){
        bc = GetComponent<BoxCollider>();
        mr = GetComponent<MeshRenderer>();
    }

    void Update(){
        
        bc.enabled = !isGreen;
        mr.material = isGreen ? redMat : greenMat;
    }
}
