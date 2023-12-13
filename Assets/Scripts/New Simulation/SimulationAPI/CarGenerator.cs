using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace SimulationAPI
{
    public class CarGeneration
    {
        // public GameObject rootCar;
        // public EffeciencyStats carStats;
        public float spawningChance = 0.3f; // between 0-1
        public List<int> entrancePositionChance = new List<int>(6);
        public List<int> exitPositionChance = new List<int>(6);
        public float intervalInSeconds = 1.0f;
        private float secondsSinceLastInterval = 0.0f;
        private int uuidCounter = 0;
        private Vector2[] positions = {
                new Vector2(-26.5f, -31.0f),
                new Vector2(-60.0f, -2.7f),
                new Vector2(-32.5f, 31.0f),
                new Vector2(26.5f, 31.0f),
                new Vector2(60.0f, 3.0f),
                new Vector2(32.5f, -31.0f)
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

        void Update(float dt, Random rand)
        {
            secondsSinceLastInterval += dt;
            if (secondsSinceLastInterval >= intervalInSeconds)
            {
                secondsSinceLastInterval = 0;

                int randomChanceValue = rand.Next();

                if (randomChanceValue < spawningChance)
                {
                    SpawnCar();
                }
            }
        }

        Car SpawnCar()
        {
            int entranceIndex = GetPositionFromChance(entrancePositionChance);
            int exitIndex = GetPositionFromChance(exitPositionChance, entranceIndex);
            char nextAction = instructions[entranceIndex, exitIndex];

            if (entranceIndex >= exitIndex)
            {
                exitIndex++;
            }

            Car newCar = new Car
            {
                UUID = uuidCounter,
                pos = positions[entranceIndex],
                orientation = rotations[entranceIndex],
                exitIndex = exitIndex,
                nextAction = nextAction
            };
            uuidCounter++;

            if (nextAction == 'l')
            {
                newCar.pos += newCar.right * -3;
            }
            return newCar;
        }

        int GetPositionFromChance(List<int> chances, int ignore = -1, Random rand)
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
                Console.WriteLine("chance is incorrectly distributed, because there is no possible position.");
            }
            return -1;
        }
    }
}