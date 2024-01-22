using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SimulationAPI
{
    public class Intersection
    {
        int[][] phases = new int[5][];
        public enum States
        {
            Green,
            Red,
            ClearingDelay,
        }
        private States state = States.Green;
        [NonSerialized] private Simulator sim;
        public Vector2 pos;
        public List<Light> lights;

        private float maxGreenTime = 7;

        private float timer;

        private int currentPhase = 0;
        private int nextPhase = 0;

        public int carStillCount = 0, carMoveCount = 0, carExitCount = 0;

        private float[,] phaseSwitches = new float[,]{
            {0.0f, 12.0f, 9.0f,  6.0f},
            {6.0f,  0.0f, 9.0f, 12.0f},
            {9.0f,  6.0f, 0.0f, 12.0f},
            {9.0f, 12.0f, 6.0f,  0.0f}
        };

        public Intersection(Simulator parent, Vector2 pos, Vector2[] lightPositions, float[] lightOrientations, bool isOn = false)
        {
            phases[0] = new int[] { 1, 5 };
            phases[1] = new int[] { 0, 4 };
            phases[2] = new int[] { 3, 7 };
            phases[3] = new int[] { 2, 6 };
            phases[4] = new int[] { };

            this.sim = parent;
            this.pos = pos;
            this.lights = new List<Light>();
            for (int i = 0; i < 8; i++)
            {
                this.lights.Add(new Light());
                this.lights[i].isOn = isOn;
                this.lights[i].pos = lightPositions[i] + this.pos;
                this.lights[i].orientation = lightOrientations[i];
            }
            timer = maxGreenTime;
        }

        public void Update(float dt, bool updateLights)
        {
            if (updateLights)
            {
                switch (state)
                {
                    case States.Green:
                        {
                            timer -= dt;
                            if (timer <= 0 || phaseIsEmpty(currentPhase))
                            {
                                NextPhase();
                            }
                            break;
                        }
                    case States.Red:
                        {
                            break;
                        }
                    case States.ClearingDelay:
                        {
                            timer -= dt;
                            if (timer <= 0)
                            {
                                setStateGreen(nextPhase);
                            }
                            break;
                        }
                }
            }
            int tmpWaitingCars, tmpDrivingCars;
            (tmpWaitingCars, tmpDrivingCars) = getCarCounts(0);
            carStillCount += tmpWaitingCars;
            carMoveCount += tmpDrivingCars;
        }

        public void setStateGreen(int phase)
        {
            state = States.Green;
            currentPhase = phase;
            ChangeSignalFase(phase);
            timer = maxGreenTime;
        }

        bool laneIsEmpty(int lightIndex)
        {
            Light l = lights[lightIndex];
            foreach (Car car in sim.cars)
            {
                if (sim.GetDistanceToCar(l.pos, l.forward, car) <= 10f)
                {
                    return false;
                }
            }
            return true;
        }

        bool phaseIsEmpty(int phaseIndex)
        {
            int[] phase = phases[phaseIndex];
            foreach (int laneIndex in phase)
            {
                if (!laneIsEmpty(laneIndex))
                {
                    return false;
                }
            }
            return true;
        }

        bool NextPhase()
        {
            nextPhase = currentPhase;
            state = States.ClearingDelay;

            for (int i = 0; i < 4; i++)
            {
                nextPhase = (nextPhase + 1) % 4;
                if (!phaseIsEmpty(nextPhase))
                {
                    timer = phaseSwitches[currentPhase, nextPhase] / 4;
                    ChangeSignalFase(4);
                    return true;
                }
            }
            return false;
        }

        public void ChangeSignalFase(int phase)
        {


            if (phase != -1)
            {
                for (int i = 0; i < 8; i++)
                {
                    lights[i].isOn = false;
                }

                foreach (int i in phases[phase])
                {
                    lights[i].isOn = true;
                }
            }
            return;
        }

        public int[] GetQueueLenghts(float maxSpeed = 0)
        {
            float maxQueueLength = 20;  //! this might have to be changed later

            int[] queueLenghts = new int[this.lights.Count];
            for (int i = 0; i < this.lights.Count; i++)
            {
                int waitingCars = 0;
                foreach (Car car in sim.cars)
                {
                    int drivingCars;
                    (waitingCars, drivingCars) = laneQueueLength(i, maxSpeed);
                    if (car.velocity <= maxSpeed)
                    {
                        float distance = sim.GetDistanceToCar(this.lights[i].pos, this.lights[i].forward, car);
                        if (distance < maxQueueLength)
                        {
                            waitingCars++;
                        }
                    }
                }

                queueLenghts[i] = waitingCars;
            }

            return queueLenghts;
        }

        public (int, int) laneQueueLength(int light, float maxSpeed)
        {
            float maxQueueLength = 20;  //! this might have to be changed later
            int waitingCars = 0;
            int drivingCars = 0;
            foreach (Car car in sim.cars)
            {
                float distance = sim.GetDistanceToCar(this.lights[light].pos, this.lights[light].forward, car);
                if (distance < maxQueueLength)
                {
                    if (car.velocity <= maxSpeed)
                    {
                        waitingCars++;
                    }
                    else
                    {
                        drivingCars++;
                    }
                }
            }
            return (waitingCars, drivingCars);
        }

        public (int, int) getCarCounts(float maxSpeed = 0)
        {
            int waitingCars = 0;
            int drivingCars = 0;
            for (int i = 0; i < this.lights.Count; i++)
            {
                int tmpWaitingCars = 0;
                int tmpDrivingCars = 0;
                (tmpWaitingCars, tmpDrivingCars) = laneQueueLength(i, maxSpeed);
                waitingCars += tmpWaitingCars;
                drivingCars += tmpDrivingCars;
            }
            return (waitingCars, drivingCars);
        }

        public List<float> GetTrafficState()
        {
            List<float> list = new List<float>();
            for (int i = 0; i < 8; i++)
            {

                list.Add(this.lights[i].isOn ? 1.0f : 0.0f);
            }
            return list;
        }

        public float CalculateReward()
        {
            float carExitReward = 1.0f;
            float carStillReward = -0.01f; //per step so actually 0.01 * 20 = 0.2

            float reward = carExitCount * carExitReward + carStillCount * carStillReward;

            carExitCount = 0;
            carStillCount = 0;
            carMoveCount = 0;

            return reward;
        }

    }

}