using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float forwardSpeed = 5.0f;
    public float rightCorneringSpeed = 3.0f;
    public float rightTurningSpeed = 3.0f;
    private char action = 'a'; // a: arrive | f: forward | l: left | r: right | e: exit
    private Quaternion targetRotation;
    private Vector3 direction;
    private Vector3 carPosition;

    void Start() {
        direction = transform.forward;
    }

    // Update is called once per frame
    void Update() {
        direction = transform.forward;
        carPosition = transform.position;

        if(action == 'a' && carPosition.x > -14) {
            action = 'r';
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90.0f, 0);
        }

        if(action == 'r') {
            // if((actionStartTime - Time.time) * rightTurningSpeed < -90) {
            //     Debug.Log("Car turn 90deg");
            //     // transform.rotation = RoundRotation(transform.rotation);
            //     action = 'e';
            // }
            transform.Translate(direction * rightCorneringSpeed * Time.deltaTime, Space.World);        
            // transform.Rotate(0, rightTurningSpeed * Time.deltaTime, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rightTurningSpeed);
            
        } else {
            transform.Translate(direction * forwardSpeed * Time.deltaTime, Space.World);        
        }
    }

}