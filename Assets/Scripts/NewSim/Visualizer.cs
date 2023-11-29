using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulator;

public class Visualizer : MonoBehaviour
{
    public GameObject Car;
    public Transform CarParent;
    public Transform trafficLights;

    public void SetVisuals(G_sceneState sceneState)
    {
        for (int i = CarParent.childCount - 1; i >= 0; i--)
        {
            Destroy(CarParent.GetChild(i).gameObject);
        }

        foreach (G_Car car in sceneState.cars)
        {
            Instantiate(Car, new Vector3(car.pos.x, 0, car.pos.y), Quaternion.Euler(new Vector3(0, car.orientation, 0)), CarParent);
        }

        List<Transform> intersections = new List<Transform> { trafficLights.GetChild(0), trafficLights.GetChild(1) };

        for (int i = 0; i < 8; i++)
        {
            int dualI = Mathf.FloorToInt(i / 2);
            int otherI = i % 2 == 0 ? 0 : 1;

            intersections[0].GetChild(dualI).GetChild(otherI).GetComponent<TrafficLight>().isGreen = sceneState.lights.cross1[i];
            intersections[1].GetChild(dualI).GetChild(otherI).GetComponent<TrafficLight>().isGreen = sceneState.lights.cross2[i];
        }
    }
}
