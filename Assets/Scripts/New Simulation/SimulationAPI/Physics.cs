using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationAPI
{
    public class Physics
    {
        Simulator super;

        public Physics(Simulator super)
        {
            this.super = super;
        }

        public bool Raycast(Vector2 origin, Vector2 dir, float maxDist, out RayHit hit, int ignore = -1)
        {
            hit = new RayHit();
            hit.maxDist = maxDist;
            hit.dist = maxDist;
            hit.car = null;
            hit.light = null;
            List<Car> carList = new List<Car>(super.cars);
            for (int i = carList.Count - 1; i >= 0; i--)
            {
                if (Vector2.Distance(origin, carList[i].pos) > maxDist || carList[i].UUID == ignore)
                {
                    carList.RemoveAt(i);  // ignore all cars too far away from car or are a specified car
                }
            }

            foreach (Car car in carList) // get distance and reference to closest car
            {
                Vector2 hit1 = LineLineIntersect(origin, origin + dir, car.pos + car.forward * car.size.y * 0.5f, car.pos - car.forward * car.size.y * 0.5f) ?? Vector2.positiveInfinity;
                Vector2 hit2 = LineLineIntersect(origin, origin + dir, car.pos - car.forward * car.size.y * 0.5f - car.right * car.size.x * 0.5f, car.pos - car.forward * car.size.y + car.right * car.size.x * 0.5f) ?? Vector2.positiveInfinity;

                if (hit1 == Vector2.positiveInfinity && hit2 == Vector2.positiveInfinity)
                {
                    continue;
                }
                float dist = Math.Min(Vector2.Distance(origin, hit1), Vector2.Distance(origin, hit2));

                if (dist < hit.dist)
                {
                    hit.car = car;
                    hit.dist = dist;
                }
            }

            List<Light> lights = super.lightsC1.Concat(super.lightsC2).ToList();

            for (int i = lights.Count - 1; i >= 0; i--)
            {
                if (Vector2.Distance(origin, lights[i].pos) > maxDist || lights[i].isOn)
                {
                    lights.RemoveAt(i); //ignore all lights that are too far away or are turned on
                }
            }

            foreach (Light light in lights)
            {
                Vector2 lightHit = LineLineIntersect(origin, origin + dir, light.pos + light.right * 1.25f, light.pos - light.right * 1.25f) ?? Vector2.positiveInfinity;

                float dist = Vector2.Distance(origin, lightHit);


                if (dist < hit.dist)
                {
                    hit.car = null;
                    hit.light = light;
                    hit.dist = dist;
                }
            }

            if (hit.dist < maxDist)
            {
                return true;
            }


            return false;
        }

        /// <summary>
        /// line line intersection between two lines given two points each, line 1 extends infinitely beyond point 2
        /// </summary>
        /// <param name="line1p1">The origin of the first line</param>
        /// <param name="line1p2">The direction of the first line, in relation to the origin</param>
        /// <param name="line2p1">point 1 of the line to intersect with</param>
        /// <param name="line2p2">point 2 of the line to intersect with</param>
        /// <returns>Returns a Vector2 if hit, otherwise null</returns>

#nullable enable        //wtf is this C#??
        private Vector2? LineLineIntersect(Vector2 line1p1, Vector2 line1p2, Vector2 line2p1, Vector2 line2p2)
#nullable disable       //I hate this...
        {
            float d =
                (line1p2.x - line1p1.x) * (line2p2.y - line2p1.y) - (line1p2.y - line1p1.y) * (line2p2.x - line2p1.x);

            if (d == 0)
            {
                return null; // no intersection
            }

            float t = ((line2p1.x - line1p1.x) * (line1p2.y - line1p1.y) - (line2p1.y - line1p1.y) * (line1p2.x - line1p1.x)) / d;
            float u = ((line2p2.y - line2p1.y) * (line2p2.x - line1p1.x) + (line2p1.x - line2p2.x) * (line2p2.y - line1p1.y)) / d;

            if (u > 0 && t > 0 && t < 1)
            {
                Vector2 intersection = new Vector2(
                    line2p1.x + t * (line2p2.x - line2p1.x),
                    line2p1.y + t * (line2p2.y - line2p1.y)
                );
                return intersection;
            }
            return null; // no intersection
        }
    }
}