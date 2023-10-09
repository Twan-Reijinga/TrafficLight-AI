using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarGeneration : MonoBehaviour {
    public GameObject rootCar;
    public float spawningChange = 0.3f; // between 0-1
    public float intervalInSeconds = 1.0f;
    private float secondsSinceLastInterval = 0.0f;
    private Vector3[] positions = {
        new Vector3(-26.5f, 0.0f, -31.0f),
        new Vector3(-60.0f, 0.0f, -2.7f),
        new Vector3(-32.5f, 0.0f, 31.0f),
        new Vector3(26.5f, 0.0f, 31.0f),
        new Vector3(60.0f, 0.0f, 3.0f),
        new Vector3(32.5f, 0.0f, -31.0f)
    };
    private Quaternion[] rotations = {
        Quaternion.Euler(0.0f, 0.0f, 0.0f),
        Quaternion.Euler(0.0f, 90.0f, 0.0f),
        Quaternion.Euler(0.0f, 180.0f, 0.0f),
        Quaternion.Euler(0.0f, 180.0f, 0.0f),
        Quaternion.Euler(0.0f, -90.0f, 0.0f),
        Quaternion.Euler(0.0f, 0.0f, 0.0f)
    };
    private char[,] instructions = {
        {'l', 'f', 'r', 'r', 'r'},
        {'r', 'l', 'f', 'f', 'f'},
        {'f', 'r', 'l', 'l', 'l'},
        {'r', 'r', 'r', 'l', 'f'},
        {'f', 'f', 'f', 'r', 'l'},
        {'l', 'l', 'l', 'f', 'r'}
    };

    void Update() {
        secondsSinceLastInterval += Time.deltaTime;
        if(secondsSinceLastInterval >= intervalInSeconds) {
            secondsSinceLastInterval = 0;
            if(Random.value < spawningChange) {
                int entranceIndex = Random.Range(0, positions.Length);
                int exitIndex = Random.Range(0, positions.Length - 1);

                GameObject newCar = Instantiate(rootCar);
                newCar.transform.parent = transform;

                newCar.transform.position = positions[entranceIndex];
                newCar.transform.rotation = rotations[entranceIndex];
                
                char nextAction = instructions[entranceIndex, exitIndex];
                if(entranceIndex >= exitIndex) {
                    exitIndex++;
                }
                newCar.GetComponent<CarMovement>().exitIndex = exitIndex;

                newCar.GetComponent<CarMovement>().nextAction = nextAction;
                if(nextAction == 'l') {
                    newCar.transform.position += newCar.transform.right * -3;
                }
            }
        }
    }
}
