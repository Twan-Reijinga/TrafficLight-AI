using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float forwardSpeed = 5.0f;
    public float corneringSpeed = 3.0f;
    public float rightTurningSpeed = 0.4f;
    public float leftTurningSpeed = 0.2f;
    public float switchTurningSpeed = 0.5f;
    public float switchRotation = 20.0f;
    public GameObject actionTrigger;
    public int exitIndex;
    public char nextAction;

    private char action = 'f'; // f: forward | l: left | r: right | s: SwitchLane
    private Quaternion targetRotation;
    private Vector3 direction;
    private Vector3 carPosition;



    private void OnTriggerEnter(Collider other) {
        if(other.name == "Right-TriggerBox") {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90.0f, 0);
            action = 'r';
        } else if(other.name == "Left-TriggerBox") {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90.0f, 0);
            action = 'l';
        } else if(other.name == "SwitchLane-TriggerBox") {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - switchRotation, 0);
            action = 's';
        }
    } 

    void Update() {
        Debug.Log(exitIndex);
        Debug.Log(nextAction);
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
    
        } else if(action == 's' && transform.rotation == targetRotation) {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y +switchRotation, 0);
            action = 'f';
        } else if ((action == 'f' && transform.rotation != targetRotation) || action == 's'){
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, switchTurningSpeed);
            transform.Translate(direction * forwardSpeed * Time.deltaTime, Space.World);        
        } else {
            transform.Translate(direction * forwardSpeed * Time.deltaTime, Space.World);        
        }
    }

}