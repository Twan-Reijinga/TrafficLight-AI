using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationAPI
{
    [Serializable]
    public class Simulator
    {
        public static Simulator instance;

        public event EventHandler<WriteEventArgs> write;
        System.Random rand;
        public int seed = 0;

        public List<Car> cars = new List<Car>();
        public Intersection[] intersections = new Intersection[2];
        CarGeneration carGenerator;
        Physics physics;
        List<int> deletedCars = new List<int>();

        private Vector2[] lightPositions = {
            new Vector2( 7.5f,  3.0f),
            new Vector2( 7.5f,  0.0f),
            new Vector2(-3.0f,  7.5f),
            new Vector2( 0.0f,  7.5f),
            new Vector2(-7.5f, -3.0f),
            new Vector2(-7.5f,  0.0f),
            new Vector2( 3.0f, -7.5f),
            new Vector2( 0.0f, -7.5f)
        };

        private Vector2[] intersectionPositions = {
            new Vector2( 29.5f, 0),
            new Vector2(-29.5f, 0)
        };

        private float[] lightOrientations = {
            0  , 0  ,
            90 , 90 ,
            180, 180,
            270, 270
        };


        public Simulator(int seed)
        {
            if (instance != null)
            {
                throw new Exception("Simulator already exists");
            }
            instance = this;

            rand = new System.Random(seed);
            carGenerator = new CarGeneration(this);
            intersections[0] = new Intersection(this, intersectionPositions[0], lightPositions, lightOrientations, true);
            intersections[1] = new Intersection(this, intersectionPositions[1], lightPositions, lightOrientations, true);

            physics = new Physics(this);
        }

        public G_sceneState GetGraphicSceneState()
        {
            G_sceneState scene = new G_sceneState();

            foreach (Car car in cars)
            {
                Car gCar = new Car(car);
                scene.cars.Add(gCar);
            }

            for (int i = 0; i < 8; i++)
            {
                scene.lights.cross1.Add(intersections[0].lights[i].isOn);
                scene.lights.cross2.Add(intersections[1].lights[i].isOn);
            }

            scene.deletedCars = new List<int>(deletedCars);
            deletedCars = new List<int>();

            return scene;
        }

        public void TestPopulation()
        {
            Print("No Fuck U: TestPopulation incomplete (Simulator.cs)");
            return;
        }

        int test = 0;
        public void Step(float dt)
        {
            UpdateCarPositions(dt);
            DestroyCars();
            UpdateTrafficLights(dt);
            Car newCar = carGenerator.Update(dt, rand);
            if (newCar != null) cars.Add(newCar);
        }

        void UpdateCarPositions(float dt)
        {
            foreach (Car car in cars)
            {
                car.Move(dt, this);
            }
        }

        void DestroyCars()
        {
            for (int i = cars.Count - 1; i >= 0; i--)
            {
                if (cars[i].isDestroyed)
                {
                    // Print("BOOOOOOM: car " + cars[i].UUID + " destroyed!");
                    deletedCars.Add(cars[i].UUID);
                    cars.RemoveAt(i);
                }
            }
        }

        void UpdateTrafficLights(float dt)
        {
            intersections[0].Update(dt);
            intersections[1].Update(dt);
        }

        // private Vector3 vec2to3(Vector2 vIn) //!quick useful temp function to convert Simulation.Vector2 to UnityEngine.Vector3
        // {
        //     return new Vector3(vIn.x, 2, vIn.y);
        // }

        public float GetDistanceToCar(Vector2 origin, Vector2 direction, Car car, float maxDistance = float.PositiveInfinity)
        {

            Vector2 fr = car.pos + car.forward * car.size.y * 0.5f + car.right * car.size.x * 0.5f; //front right
            Vector2 fl = car.pos + car.forward * car.size.y * 0.5f - car.right * car.size.x * 0.5f;
            Vector2 br = car.pos - car.forward * car.size.y * 0.5f + car.right * car.size.x * 0.5f;
            Vector2 bl = car.pos - car.forward * car.size.y * 0.5f - car.right * car.size.x * 0.5f;

            // Debug.DrawLine(vec2to3(fr), vec2to3(fl), Color.red, 0.05f);
            // Debug.DrawLine(vec2to3(fr), vec2to3(br), Color.red, 0.05f);
            // Debug.DrawLine(vec2to3(bl), vec2to3(fl), Color.red, 0.05f);
            // Debug.DrawLine(vec2to3(br), vec2to3(bl), Color.red, 0.05f);
            // Debug.DrawLine(vec2to3(origin), vec2to3(origin + direction * 10), Color.red, 0.05f); //!bunch of test drawLines

            Vector2 hitF = Physics.LineLineIntersect(origin, origin + direction, fr, fl) ?? Vector2.positiveInfinity; //front
            Vector2 hitB = Physics.LineLineIntersect(origin, origin + direction, br, bl) ?? Vector2.positiveInfinity; //back
            Vector2 hitR = Physics.LineLineIntersect(origin, origin + direction, fr, br) ?? Vector2.positiveInfinity; //left
            Vector2 hitL = Physics.LineLineIntersect(origin, origin + direction, fl, bl) ?? Vector2.positiveInfinity; //right

            Vector2[] hits = new Vector2[] { hitF, hitB, hitR, hitL };

            float closestDistance = float.PositiveInfinity;
            foreach (Vector2 carHit in hits)
            {
                if (carHit == Vector2.positiveInfinity) continue;
                float distance = Vector2.Distance(origin, carHit);
                if (distance < closestDistance && distance < maxDistance)
                {
                    closestDistance = distance;
                }
            }
            return closestDistance;
        }

        public bool Raycast(Vector2 origin, Vector2 dir, float maxDist, out RayHit hit, int ignore = -1)
        {
            hit = new RayHit();
            hit.maxDist = maxDist;
            hit.dist = maxDist;
            hit.car = null;
            hit.light = null;
            List<Car> carList = new List<Car>(cars);
            for (int i = carList.Count - 1; i >= 0; i--)
            {
                if (Vector2.Distance(origin, carList[i].pos) > maxDist || carList[i].UUID == ignore)
                {
                    carList.RemoveAt(i);  // ignore all cars too far away from car or are a specified car
                }
            }

            foreach (Car car in carList) // get distance and reference to closest car
            {
                float dist = GetDistanceToCar(origin, dir, car, maxDist);
                if (dist < hit.dist)
                {
                    hit.car = car;
                    hit.dist = dist;
                }
            }

            List<Light> lights = intersections[0].lights.Concat(intersections[1].lights).ToList();

            for (int i = lights.Count - 1; i >= 0; i--)
            {
                if (Vector2.Distance(origin, lights[i].pos) > maxDist || lights[i].isOn)
                {
                    lights.RemoveAt(i); //ignore all lights that are too far away or are turned on
                }
            }

            foreach (Light light in lights)
            {
                Vector2 lightHit = Physics.LineLineIntersect(origin, origin + dir, light.pos + light.right * 1.25f, light.pos - light.right * 1.25f) ?? Vector2.positiveInfinity;

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

        public void Print(string e)
        {
            write?.Invoke(this, new WriteEventArgs(e));
        }

        public bool SpawnCar(int entranceindex, int exitIndex)
        {
            try
            {
                cars.Add(carGenerator.SpawnCar(entranceindex, exitIndex));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
