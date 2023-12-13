using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace SimulationAPI
{
    public class Simulator
    {
        Random rand;
        public int seed = 0;
        List<Car> cars = new List<Car>();
        List<Light> lightsC1 = new List<Light>();
        List<Light> lightsC2 = new List<Light>();
        private Vector2[] lightPositions = {
            new Vector2(7.5f, 3.0f),
            new Vector2(7.5f, 0.0f)
        };

        public Simulator(int seed)
        {
            rand = new Random(seed);
        }

        // private Vector2[] spawnPositions = {
        //     new Vector2(-26.5f, -31.0f),
        //     new Vector2(-60.0f, -2.7f),
        //     new Vector2(-32.5f, 31.0f),
        //     new Vector2(26.5f, 31.0f),
        //     new Vector2(60.0f, 3.0f),
        //     new Vector2(32.5f, -31.0f)
        // };

        // private float[] spawnOrientations = {
        //     0.0f,
        //     90.0f,
        //     180.0f,
        //     180.0f,
        //     -90.0f,
        //     0.0f
        // };


        public G_sceneState GetGraphicSceneState()
        {
            G_sceneState scene = new G_sceneState();

            foreach (Car car in cars)
            {
                G_Car g_Car = new G_Car();
                g_Car.UUID = car.UUID;
                g_Car.pos = car.pos;
                g_Car.orientation = car.orientation;
                scene.cars.Add(g_Car);
            }

            for (int i = 0; i < 8; i++)
            {
                scene.lights.cross1.Add(lightsC1[i].isOn);
                scene.lights.cross2.Add(lightsC2[i].isOn);
            }

            return scene;
        }

        // public Car GenerateCar(int id)
        // {
        //     // int index = (int)Mathf.Floorint)Mathf.Floor(Random.Range(0, spawnPositions.Length));

        //     int entranceIndex = GetPositionFromChance(entrancePositionChance);

        //     Car car = new Car
        //     {
        //         orientation = spawnOrientations[index],
        //         pos = spawnPositions[index],
        //         UUID = id
        //     };

        //     if (false)
        //     { //car goes to left in first time
        //         car.pos += car.right * -3;
        //     }
        //     return car;
        // }

        public void TestPopulation()
        {
            return;
            cars = new List<Car>();
            int amountOfCars = 50;
            for (int i = 0; i < amountOfCars; i++)
            {
                // Car car = GenerateCar(i);
                // cars.Add(car);
            }

        }

        public void Step(float dt)
        {
            UpdateCarPositions(dt);
            UpdateTrafficLights(dt);
        }

        void UpdateCarPositions(float dt)
        {
            foreach (Car car in cars)
            {
                // car.Move(dt); // TEST SPEED OF 3!!

            }
        }

        void UpdateTrafficLights(float dt)
        {

        }

        public bool Raycast(Vector2 origin, Vector2 dir, float maxDist, out RayHit hit, int ignore = -1)
        {
            hit = new RayHit();
            hit.dist = maxDist;
            List<Car> carList = new List<Car>(cars);
            for (int i = carList.Count - 1; i >= 0; i--)
            {
                if (Vector2.Distance(origin, carList[i].pos) > maxDist || carList[i].UUID == ignore)
                {
                    carList.RemoveAt(i);  // ignore all cars too far away from car
                }
            }
            //carList.Sort((a, b) => Vector2.Distance(a.pos, origin).CompareTo(Vector2.Distance(b.pos, origin))); // sort cars based on distance to origin so to make first car that hits the closest one

            foreach (Car car in carList) // get distance and reference to closest car
            {
                Vector2 hit1 = LineLineIntersect(origin, dir, car.forward * car.size.y + car.right * car.size.x, Vector2.zero - car.forward * car.size.y - car.right * car.size.x) ?? Vector2.positiveInfinity;
                Vector2 hit2 = LineLineIntersect(origin, dir, car.forward * car.size.y - car.right * car.size.x, Vector2.zero - car.forward * car.size.y + car.right * car.size.x) ?? Vector2.positiveInfinity;

                if (hit1 == Vector2.positiveInfinity && hit2 == Vector2.positiveInfinity)
                {
                    continue;
                }
                float dist = Math.Min(Vector2.Distance(origin, hit1), Vector2.Distance(origin, hit2));

                if (hit.dist < dist)
                {
                    hit.car = car;
                    hit.dist = dist;
                }
            }

            if (hit.dist != maxDist)
            {
                return true;
            }


            return false;
        }

        private static Vector2? LineLineIntersect(Vector2 from1, Vector2 to1, Vector2 from2, Vector2 to2)
        {
            float d =
                (to1.x - from1.x) * (to2.y - from2.y) - (to1.y - from1.y) * (to2.x - from2.x);

            if (d == 0)
            {
                return null; // no intersection
            }

            float t =
                ((from2.x - from1.x) * (to1.y - from1.y) - (from2.y - from1.y) * (to1.x - from1.x)) / d;
            float u =
                 ((to2.y - from2.y) * (to2.x - from1.x) + (from2.x - to2.x) * (to2.y - from1.y)) / d;

            if (u > 0 && t > 0 && t < 1)
            {
                Vector2 intersection = new Vector2(
                    from2.x + t * (to2.x - from2.x),
                    from2.y + t * (to2.y - from2.y)
                );
                return intersection;
            }
            return null; // no intersection
        }
    }
}