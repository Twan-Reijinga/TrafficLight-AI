using System;
using System.Collections.Generic;

namespace SimulationAPI
{
    public class Simulator
    {
        public event EventHandler<WriteEventArgs> write;

        System.Random rand;
        public int seed = 0;

        public List<Car> cars = new List<Car>();
        public List<Light> lightsC1 = new List<Light>();
        public List<Light> lightsC2 = new List<Light>();

        CarGeneration carGenerator;
        Physics physics;

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

            return scene;
        }

        public void TestPopulation()
        {
            Print("No Fuck U");
            return;
        }

        public void Step(float dt)
        {
            UpdateCarPositions(dt);
            UpdateTrafficLights(dt);
            Car newCar = carGenerator.Update(dt, rand);
            if (newCar != null) cars.Add(newCar);
        }

        void UpdateCarPositions(float dt)
        {
            foreach (Car car in cars)
            {
                car.Move(dt, physics); // TEST SPEED OF 3!!
                // Print("current: " + car.currentAction);
                // Print("next:    " + car.nextAction);
            }
        }

        void UpdateTrafficLights(float dt)
        { }

        public void Print(string e)
        {
            write?.Invoke(this, new WriteEventArgs(e));
        }
    }
}