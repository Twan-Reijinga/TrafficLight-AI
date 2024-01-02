using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimulationAPI;
using TMPro;
using System;

public class CarInfoVisualizer : MonoBehaviour
{
    public GameObject infoBlock;

    public Transform infoParent;
    public Transform carParent;
    void Update()
    {
        foreach (Transform child in infoParent)
        {
            Destroy(child.gameObject);
        }
        GameObject closest = GetClosest();
        if (closest != null)
        {
            MakeInfoObject(closest.GetComponent<SceneCar>());
        }
    }

    GameObject GetClosest()
    {
        GameObject closestObject = null;
        float closestDist = float.MaxValue;
        foreach (Transform car in carParent)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(car.position);
            float dist = UnityEngine.Vector2.Distance(screenPos, new UnityEngine.Vector2(Screen.width / 2f, Screen.height / 2f));

            if (dist < closestDist)
            {
                closestDist = dist;
                closestObject = car.gameObject;
            }
        }
        return closestObject;
    }

    GameObject MakeInfoObject(SceneCar sCar)
    {
        GameObject info = Instantiate(infoBlock, Camera.main.WorldToScreenPoint(sCar.transform.position), Quaternion.identity, infoParent);

        Car car = sCar.car;

        string text = car.UUID.ToString() + "<br>" + Math.Round(car.orientation).ToString() + "<br>" + "<br>" + car.currentAction + "<br>" + car.nextAction + "<br>" + "<br>" + car.exitIndex.ToString() + "<br>" + Math.Round(car.velocity, 2).ToString() + "<br><br>" + (car.isDestroyed ? "t" : "f");

        info.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        return info;
    }
}
