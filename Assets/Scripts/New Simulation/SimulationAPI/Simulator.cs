using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationAPI
{
    public class Simulator
    {
        public event EventHandler<WriteEventArgs> write;
        Replay replay = new Replay();
        System.Random rand;
        public int seed = 0;

        public List<Car> cars = new List<Car>();
        public List<Light> lightsC1 = new List<Light>();
        public List<Light> lightsC2 = new List<Light>();

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
            rand = new System.Random(seed);
            carGenerator = new CarGeneration(this);
            for (int i = 0; i < 8; i++)
            {
                lightsC1.Add(new Light { isOn = true, pos = lightPositions[i] + intersectionPositions[0], orientation = lightOrientations[i] });
                lightsC2.Add(new Light { isOn = true, pos = lightPositions[i] + intersectionPositions[1], orientation = lightOrientations[i] });
            }

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
                scene.lights.cross1.Add(lightsC1[i].isOn);
                scene.lights.cross2.Add(lightsC2[i].isOn);
            }

            scene.deletedCars = new List<int>(deletedCars);

            return scene;
        }

        public void TestPopulation()
        {
            Print("No Fuck U: TestPopulation incomplete (Simulator.cs)");
            return;
        }

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
            deletedCars = new List<int>();
            for (int i = cars.Count - 1; i > 0; i--)
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
        { }

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
                Vector2 fr = car.pos + car.forward * car.size.y * 0.5f + car.right * car.size.x * 0.5f; //front right
                Vector2 fl = car.pos + car.forward * car.size.y * 0.5f - car.right * car.size.x * 0.5f;
                Vector2 br = car.pos - car.forward * car.size.y * 0.5f + car.right * car.size.x * 0.5f;
                Vector2 bl = car.pos - car.forward * car.size.y * 0.5f - car.right * car.size.x * 0.5f;

                Vector2 hitF = Physics.LineLineIntersect(origin, origin + dir, fr, fl) ?? Vector2.positiveInfinity; //front
                Vector2 hitB = Physics.LineLineIntersect(origin, origin + dir, br, bl) ?? Vector2.positiveInfinity; //back
                Vector2 hitR = Physics.LineLineIntersect(origin, origin + dir, fr, br) ?? Vector2.positiveInfinity; //left
                Vector2 hitL = Physics.LineLineIntersect(origin, origin + dir, fl, bl) ?? Vector2.positiveInfinity; //right

                Vector2[] hits = new Vector2[] { hitF, hitB, hitR, hitL };

                foreach (Vector2 carHit in hits)
                {
                    if (carHit == Vector2.positiveInfinity) continue;
                    float dist = Vector2.Distance(origin, carHit);

                    if (dist < hit.dist)
                    {
                        hit.car = car;
                        hit.dist = dist;
                    }
                }
            }

            List<Light> lights = lightsC1.Concat(lightsC2).ToList();

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
    }
}
