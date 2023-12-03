using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Simulator
{
    public class G_Car
    {
        public uint UUID = 0;
        public Vector2 pos = Vector2.zero;
        public float orientation = 0;
    }

    public class G_Lights
    {
        public List<bool> cross1 = new List<bool>();
        public List<bool> cross2 = new List<bool>();

    }

    public class Car
    {
        public uint UUID = 0;
        public Vector2 pos;
        public float orientation = 0;
        public int startIndex;
        public int endIndex;
        public Vector2 forward
        {
            get
            {
                return new Vector2(Mathf.Sin(Mathf.Deg2Rad * orientation), Mathf.Cos(Mathf.Deg2Rad * orientation));
            }
        }

        public Vector2 right
        {
            get
            {
                return new Vector2(Mathf.Cos(Mathf.Deg2Rad * -orientation), Mathf.Sin(Mathf.Deg2Rad * -orientation));
            }
        }

        public void Move(float dt, float speed)
        {
            pos += forward * dt * speed;
        }

    }

    public class Light
    {
        public bool isOn = false;
    }

    public class G_sceneState
    {
        public List<G_Car> cars = new List<G_Car>(8);
        public G_Lights lights = new G_Lights();
    }

    public class Simulator
    {
        public int seed = 0;
        List<Car> cars = new List<Car>();
        List<Light> lightsC1 = new List<Light>();
        List<Light> lightsC2 = new List<Light>();

        public List<Vector2[]> spawnPositions = new List<Vector2[]>() { };
        private Vector2[] positions = {
            new Vector2(-26.5f, -31.0f),
            new Vector2(-60.0f, -2.7f),
            new Vector2(-32.5f, 31.0f),
            new Vector2(26.5f, 31.0f),
            new Vector2(60.0f, 3.0f),
            new Vector2(32.5f, -31.0f)
        };

        private float[] orientations = {
            0.0f,
            90.0f,
            180.0f,
            180.0f,
            -90.0f,
            0.0f
        };


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

        public Car GenerateCar(uint id)
        {
            int index = (int)Mathf.Floor(Random.Range(0, positions.Length));

            Car car = new Car
            {
                orientation = orientations[index],
                pos = positions[index],
                UUID = id
            };

            if (false)
            { //car goes to left in first time
                car.pos += car.right * -3;
            }
            return car;
        }

        public void TestPopulation()
        {
            Random.InitState(seed);
            cars = new List<Car>();
            int amountOfCars = 50;
            for (int i = 0; i < amountOfCars; i++)
            {
                Car car = GenerateCar((uint)i);
                cars.Add(car);
            }

            for (int i = 0; i < 8; i++)
            {
                Light light = new Light();
                light.isOn = Random.value >= 0.5f;
                lightsC1.Add(light);

                Light light2 = new Light();
                light2.isOn = Random.value >= 0.5f;
                lightsC2.Add(light2);
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
                car.Move(dt, 3); // TEST SPEED OF 3!!

            }
        }

        void UpdateTrafficLights(float dt)
        {

        }
    }
}