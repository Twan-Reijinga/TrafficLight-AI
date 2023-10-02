using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficLightsControllerNL : MonoBehaviour
{
    private List<DualTrafficLightsHolder> Lights;

    void Start(){
        Lights = GetComponentsInChildren<DualTrafficLightsHolder>().ToList<DualTrafficLightsHolder>();
        if (Lights[0].gameObject.name!= "0"){
            print("there might be an error with the traffic lights!!!");
        }
    }
}
