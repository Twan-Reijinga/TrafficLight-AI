using System.Collections;
using System.Collections.Generic;
using System.Globalization;

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

    public class Vector2
    {
        public float x = 0;
        public float y = 0;

        public Vector2(float ix, float iy)
        {
            x = ix;
            y = iy;
        }
    }
}