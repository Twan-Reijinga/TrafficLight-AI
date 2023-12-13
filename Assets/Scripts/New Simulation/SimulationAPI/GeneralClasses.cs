using System;
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
            return !(v1 == v2);
        }

        public static float Distance(Vector2 v1, Vector2 v2)
        {
            Vector2 v = v2 - v1;
            return (float)Math.Sqrt(Mathf.Pow(v.x, 2) + Mathf.Pow(v.y, 2));
        }

        public static Vector2 positiveInfinity = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        public static Vector2 zero = new Vector2(0, 0);

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return a + (b - a) * t;
        }

    }
}