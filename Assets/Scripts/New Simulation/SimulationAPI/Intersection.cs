using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public Simulator sim;
        public Vector2 pos;
        public List<Light> lights;

        private float maxGreenTime = 7;

        private float timer;

        private int currentPhase = 0;
        private int nextPhase = 0;

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

        public void Update(float dt)
        {
            switch (state)
            {
                case States.Green:
                    {
                        timer -= dt;
                        if (timer <= 0 || NoCarsAtPhases()[currentPhase])
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
                        timer -= dt * 10000;
                        if (timer <= 0)
                        {
                            setStateGreen(nextPhase);
                        }
                        break;
                    }
            }
        }

        public void setStateGreen(int phase)
        {
            state = States.Green;
            currentPhase = phase;
            ChangeSignalFase(phase);
            timer = maxGreenTime;
        }

        bool[] NoCarsAtPhases()
        {
            bool[] list = new bool[4];
            int[] queues = GetQueueLenghts();
            for (int i = 0; i < 4; i++)
            {
                int[] phase = phases[i];
                bool isClear = false;
                foreach (int index in phase)
                {
                    if (queues[index] == 0)
                    {
                        isClear = true;
                    }
                }
                list[i] = isClear;
            }
            return list;
        }

        bool NextPhase()
        {
            nextPhase = currentPhase;
            state = States.ClearingDelay;

            bool[] nocarsatphases = NoCarsAtPhases();
            for (int i = 0; i < 4; i++)
            {
                nextPhase = (nextPhase + 1) % 4;
                if (!nocarsatphases[i])
                {
                    timer = 3;  //!temp length change to calculation
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
                // if(waitingCars > 0) 
                // {
                // Print("Light " + i + " at (" + lights[i].pos.x + ", " + lights[i].pos.y + ") has " + waitingCars + " waiting car(s)");
                // }
            }

            return queueLenghts;
        }

        public List<float> GetTrafficLightAIInputs()
        {
            List<float> list = new List<float>();
            for (int i = 0; i < 8; i++)
            {

                list.Add(this.lights[i].isOn ? 1.0f : 0.0f);
            }
            return list;
        }

    }

}