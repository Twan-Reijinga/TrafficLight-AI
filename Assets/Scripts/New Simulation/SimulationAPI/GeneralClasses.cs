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

        public Vector2(float X, float Y)
        {
            x = X;
            y = Y;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2 operator +(Vector2 v, float f)
        {
            return new Vector2(v.x + f, v.y + f);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2 operator -(Vector2 v, float f)
        {
            return new Vector2(v.x - f, v.y - f);
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x * v2.x, v1.y * v2.y);
        }

        public static Vector2 operator *(Vector2 v, float f)
        {
            return new Vector2(v.x * f, v.y * f);
        }

        public static Vector2 operator /(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x / v2.x, v1.y / v2.y);
        }

        public static Vector2 operator /(Vector2 v, float f)
        {
            return new Vector2(v.x / f, v.y / f);
        }

        public static bool operator ==(Vector2 v1, Vector2 v2)
        {
            return (v1.x == v2.x) && (v1.y == v2.y);
        }

        public static bool operator !=(Vector2 v1, Vector2 v2)
        {
            return !(v1 == v2)
        }

    }
}