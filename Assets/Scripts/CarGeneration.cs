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
        new Vector3(-60.0f, 0.0f, -2.7f)
    };
    private Quaternion[] rotations = {
        Quaternion.Euler(0.0f, 0.0f, 0.0f),
        Quaternion.Euler(0.0f, 90.0f, 0.0f)
    };
    // private Char[,] actions {
    //     {'l', 'f', 'r', 'r', 'r'},
    //     {'r', 'l', 'f', 'f', 'f'},
    //     {'f', 'r', 'l', 'l', 'l'},
    //     {'r', 'r', 'r', 'l', 'f'},
    //     {'f', 'f', 'f', 'r', 'l'},
    //     {'l', 'l', 'l', 'f', 'r'},
    // } 

    void Update() {
        secondsSinceLastInterval += Time.deltaTime;
        Debug.Log(secondsSinceLastInterval);
        if(secondsSinceLastInterval >= intervalInSeconds) {
            secondsSinceLastInterval = 0;
            if(Random.value < spawningChange) {
                int index = Random.Range(0, positions.Length);

                GameObject newCar = Instantiate(rootCar);
                newCar.transform.parent = transform;

                newCar.transform.position = positions[index];
                newCar.transform.rotation = rotations[index];

            }
        }
    }
}
