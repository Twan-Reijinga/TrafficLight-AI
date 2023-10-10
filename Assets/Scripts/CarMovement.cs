using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speedMult;
    public float speed;
    public float forwardSpeed = 5.0f;
    public float corneringSpeed = 3.0f;
    public float rightTurningSpeed = 0.4f;
    public float leftTurningSpeed = 0.2f;
    public float switchTurningSpeed = 0.5f;
    public float switchRotation = 20.0f;
    public int exitIndex;
    public char nextAction;

    private char action = 'f'; // f: forward | l: left | r: right | s: SwitchLane
    private Quaternion targetRotation;
    private Vector3 direction;
    private Vector3 carPosition;

    private void Start()
    {
        speed = forwardSpeed * speedMult;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Right-TriggerBox" && nextAction == 'r' && action == 'f')
        {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90.0f, 0);
            speed = corneringSpeed * speedMult;
            action = 'r';
        }
        else if (other.name == "Left-TriggerBox" && nextAction == 'l' && action == 'f')
        {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90.0f, 0);
            speed = corneringSpeed * speedMult;
            action = 'l';
        }
        else if (other.name == "SwitchLane-TriggerBox" && action == 'f') {
            if (exitIndex == 1 || exitIndex == 4)
            {
                targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - switchRotation, 0);
                speed = forwardSpeed * speedMult;
                action = 's';
                nextAction = 'l';
            }
            else if (exitIndex == 2 || exitIndex == 5)
            {
                nextAction = 'f';
            }
            else if (exitIndex == 3 || exitIndex == 6)
            {
                nextAction = 'r';
            }
        }
        else if (other.name == "Exit-TriggerBox") {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        direction = transform.forward;
        carPosition = transform.position;

        if (action == 'r')
        {
            transform.Translate(direction * speed * speedMult * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rightTurningSpeed * speedMult * speedMult * Time.deltaTime);
            if (transform.rotation == targetRotation)
            {
                speed = forwardSpeed * speedMult;
                action = 'f';
            }
        }
        else if (action == 'l')
        {
            transform.Translate(direction * speed * speedMult * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, leftTurningSpeed * speedMult * speedMult * Time.deltaTime);
            if (transform.rotation == targetRotation)
            {
                speed = forwardSpeed * speedMult;
                action = 'f';
            }

        }
        else if (action == 's' && transform.rotation == targetRotation)
        {
            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + switchRotation, 0);
            speed = forwardSpeed * speedMult;
            action = 'f';
        }
        else if ((action == 'f' && transform.rotation != targetRotation) || action == 's')
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, switchTurningSpeed * speedMult * speedMult * Time.deltaTime);
            transform.Translate(direction * speed * speedMult * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(direction * speed * speedMult * Time.deltaTime, Space.World);
        }
    }

}