using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float forwardSpeed = 5.0f;
    public float corneringSpeed = 3.0f;
    public float rightTurningSpeed = 0.4f;
    public float leftTurningSpeed = 0.2f;
    public GameObject actionTrigger;

    private char action = 'f'; // f: forward | l: left | r: right
    private Quaternion targetRotation;
    private Vector3 direction;
    private Vector3 carPosition;


    // Update is called once per frame
    private void OnTriggerEnter(Collider other) {
        if(other.name == "Forward-Right-TriggerBox") {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90.0f, 0);
            action = 'r';
        } else if(other.name == "Left-TriggerBox") {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y -90.0f, 0);
            action = 'l';
        }
    } 

    void Update() {
        direction = transform.forward;
        carPosition = transform.position;

        if(action == 'r') {
            transform.Translate(direction * corneringSpeed * Time.deltaTime, Space.World);        
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rightTurningSpeed);
            if(transform.rotation == targetRotation) {
                action = 'f';
            }
        } else if (action == 'l') {
            transform.Translate(direction * corneringSpeed * Time.deltaTime, Space.World);        
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, leftTurningSpeed);
            if(transform.rotation == targetRotation) {
                action = 'f';
            }
    
        } else{
            transform.Translate(direction * forwardSpeed * Time.deltaTime, Space.World);        
        }
    }

}