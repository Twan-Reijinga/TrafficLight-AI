using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SimulationAPI
{
    public class RayHit
    {
        public Light light;
        public Car car;
        public bool hasHit;
        public float dist = float.MaxValue;
    } 

    public class Light
    {
        public bool isOn = false;
        public Vector2 pos;
        public float orientation;
    }
}