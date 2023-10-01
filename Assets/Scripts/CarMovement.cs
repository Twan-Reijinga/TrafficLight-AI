using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float forwardSpeed = 5.0f;
    public float rightCorneringSpeed = 3.0f;
    public float rightTurningSpeed = 3.0f;
    public GameObject actionTrigger;

    private char action = 'a'; // a: arrive | f: forward | l: left | r: right | e: exit
    private Quaternion targetRotation;
    private Vector3 direction;
    private Vector3 carPosition;


    // Update is called once per frame
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject == actionTrigger) {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90.0f, 0);
            action = 'r';
        }
    } 

    void Update() {
        direction = transform.forward;
        carPosition = transform.position;

        if(action == 'r') {
            transform.Translate(direction * rightCorneringSpeed * Time.deltaTime, Space.World);        
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rightTurningSpeed);
        } else {
            transform.Translate(direction * forwardSpeed * Time.deltaTime, Space.World);        
        }
    }

}