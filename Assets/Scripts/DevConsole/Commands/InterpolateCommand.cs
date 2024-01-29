using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Interpolate Command", menuName = "Utilities/DevConsole/Commands/Interpolate")]
public class InterpolateCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        if (args.Length == 1)
        {
            if (bool.TryParse(args[0], out bool result))
            {
                SimulationController.instance.visualiser.interpolate = result;
                return true;
            }
        }
        return false;
    }
}