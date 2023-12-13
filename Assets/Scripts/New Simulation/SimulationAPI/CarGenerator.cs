using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SimulationAPI
{
    public class CarGeneration : MonoBehaviour
    {
        public GameObject rootCar;
        public EffeciencyStats carStats;
        public float spawningChance = 0.3f; // between 0-1
        public List<int> entrancePositionChance = new List<int>(6);
        public List<int> exitPositionChance = new List<int>(6);
        public float intervalInSeconds = 1.0f;
        private float secondsSinceLastInterval = 0.0f;
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

        void Update(dt)
        {
            secondsSinceLastInterval += dt;
            if (secondsSinceLastInterval >= intervalInSeconds)
            {
                secondsSinceLastInterval = 0;
                if (Random.value < spawningChance)
                {
                    int entranceIndex = GetPositionFromChance(entrancePositionChance); 
                    // Random.Range(0, positions.Length);
                    int exitIndex = GetPositionFromChance(exitPositionChance, entranceIndex);
                    // Random.Range(0, positions.Length - 1);
                    // print(entranceIndex + " - " + exitIndex);

                    GameObject newCar = Instantiate(rootCar);
                    newCar.transform.parent = transform;

                    newCar.transform.position = positions[entranceIndex];
                    newCar.transform.rotation = rotations[entranceIndex];

                    char nextAction = instructions[entranceIndex, exitIndex];
                    if (entranceIndex >= exitIndex)
                    {
                        exitIndex++;
                    }
                    newCar.GetComponent<CarMovement>().exitIndex = exitIndex;
                    newCar.GetComponent<CarMovement>().carStats = carStats;

                    newCar.GetComponent<CarMovement>().nextAction = nextAction;
                    if (nextAction == 'l')
                    {
                        newCar.transform.position += newCar.transform.right * -3;
                    }
                }
            }
        }

        int GetPositionFromChance(List<int> chances, int ignore = -1) {
            List<int> modifiedChances = new List<int>(chances);
            if(ignore >= 0) {
                modifiedChances.RemoveAt(ignore);
            }
            // string items = "";
            // foreach(var item in modifiedChances) {
            //     items += item + ", ";
            // }
            // print(items);
            int totalChance = modifiedChances.Sum();
            int randomChoice = Random.Range(0, totalChance);
            for (int i = 0; i < modifiedChances.Count; i++){
                if(modifiedChances[i] > randomChoice) {
                    return i;
                }
                randomChoice -= modifiedChances[i];
            }
            if(totalChance == 0) {
                print("chance is incorrectly distributed, because there is no possible position.");
            } 
            return -1;
        }
    }
}