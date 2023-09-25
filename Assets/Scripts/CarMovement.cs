using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public string action = "straight"; // left | straight | right
    private Vector3 direction;

    void Start()
    {
        direction = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);        
    }
}