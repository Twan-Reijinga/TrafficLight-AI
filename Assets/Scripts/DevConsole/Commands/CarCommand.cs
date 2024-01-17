using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimulationAPI;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Log Command", menuName = "Utilities/DevConsole/Commands/Car")]
public class CarCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        int entranceIndex, exitIndex;
        if (int.TryParse(args[0], out entranceIndex) && int.TryParse(args[1], out exitIndex))
        {
            if (Simulator.instance.SpawnCar(entranceIndex, exitIndex))
            {
                Debug.Log("live");
                return true;
            }
            else Debug.Log("inputs incorrect!");
        }
        else
        {
            Debug.Log("inputs not ints! >:(");
        }
        Debug.Log("DIE");
        return false;
    }
}