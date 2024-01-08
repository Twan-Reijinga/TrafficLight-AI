using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationAPI
{
    public class CarGeneration
    {
        // public GameObject rootCar;
        // public EffeciencyStats carStats;
        public float spawningChance = 0.3f; // between 0-1
        public List<int> entrancePositionChance = new List<int> { 1, 1, 1, 1, 1, 1 };
        public List<int> exitPositionChance = new List<int> { 1, 1, 1, 1, 1, 1 };
        public float intervalInSeconds = 1.0f;
        private float secondsSinceLastInterval = 0.0f;
        private int uuidCounter = 0;
        private Vector2[] positions = {
                new Vector2(-26.5f, -31.0f),
                new Vector2(-60.0f, - 3.0f),
                new Vector2(-32.5f,  31.0f),
                new Vector2( 26.5f,  31.0f),
                new Vector2( 60.0f,   3.0f),
                new Vector2( 32.5f, -31.0f)
            };

        private float[] rotations = {
            0.0f,
            90.0f,
            180.0f,
            180.0f,
            -90.0f,
            0.0f
        };

        private char[,] instructions = {
            {'l', 'f', 'r', 'r', 'r'},
            {'r', 'l', 'f', 'f', 'f'},
            {'f', 'r', 'l', 'l', 'l'},
            {'r', 'r', 'r', 'l', 'f'},
            {'f', 'f', 'f', 'r', 'l'},
            {'l', 'l', 'l', 'f', 'r'}
        };
        private Simulator super;

        public CarGeneration(Simulator super)
        {
            this.super = super;
        }

        public Car Update(float dt, Random rand)
        {
            secondsSinceLastInterval += dt;
            if (secondsSinceLastInterval >= intervalInSeconds) //if can spawn car
            {
                secondsSinceLastInterval = 0.0f;

                float randomChanceValue = (float)rand.NextDouble();

                if (randomChanceValue < spawningChance)     //if chance is good
                {
                    return SpawnCar(rand);
                }
            }
            return null;
        }

        Car SpawnCar(Random rand)
        {
            int entranceIndex = GetPositionFromChance(entrancePositionChance, rand);
            int exitIndex = GetPositionFromChance(exitPositionChance, rand, entranceIndex);
            char nextAction = instructions[entranceIndex, exitIndex];

            if (entranceIndex >= exitIndex)
            {
                exitIndex++;
            }

            Car newCar = new Car(uuidCounter, positions[entranceIndex], rotations[entranceIndex], exitIndex, nextAction);
            uuidCounter++;


            if (nextAction == 'l')
            {
                newCar.pos += newCar.right * -3;
            }

            RayHit hit;
            if (super.Raycast(newCar.pos, newCar.pos + newCar.forward, 7, out hit))
            {
                super.Print("aAAA");
                super.Print(entranceIndex.ToString());
                return null;
            }

            return newCar;
        }

        int GetPositionFromChance(List<int> chances, Random rand, int ignore = -1)
        {
            List<int> modifiedChances = new List<int>(chances);
            if (ignore >= 0)
            {
                modifiedChances.RemoveAt(ignore);
            }

            int totalChance = modifiedChances.Sum();
            int randomChoice = rand.Next(0, totalChance);
            for (int i = 0; i < modifiedChances.Count; i++)
            {
                if (modifiedChances[i] > randomChoice)
                {
                    return i;
                }
                randomChoice -= modifiedChances[i];
            }
            if (totalChance == 0)
            {
                super.Print("chance is incorrectly distributed, because there is no possible position.");
            }
            return -1;
        }
    }
}