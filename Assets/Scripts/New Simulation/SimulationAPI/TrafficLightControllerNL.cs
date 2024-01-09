using System;
using System.Collections.Generic;

namespace SimulationAPI
{

    public class Intersection
    {
        public Simulator sim;
        public Vector2 pos;
        public List<Light> lights;
        public Intersection(Simulator parent, Vector2 pos, Vector2[] lightPositions, float[] lightOrientations, bool isOn = false)
        {
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
        }

        public void ChangeSignalFase(int phase)
        {
            bool[,] phases = {
                {false, false, false, false},
                {true, false, false, false},
                {false, true, false, false},
                {false, false, true, false},
                {false, false, false, true}
            };

            if (phase != -1)
            {
                float j = 0;
                for (int i = 0; i < 8; i++)
                {
                    lights[i].isOn = phases[phase, (int)Math.Floor(j)];
                    j += 0.5f;
                }
            }
            return;
        }

        public int[] GetQueueLenghts()
        {
            int[] queueLenghts = new int[this.lights.Count];
            for (int i = 0; i < this.lights.Count; i++)
            {
                int waitingCars = 0;
                foreach (Car car in sim.cars)
                {
                    float distance = sim.GetDistanceToCar(this.lights[i].pos, this.lights[i].pos + this.lights[i].forward, car);
                    if (distance < float.PositiveInfinity)
                    {
                        // TODO: only include cars that are waiting; speed = 0 //
                        waitingCars++;
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