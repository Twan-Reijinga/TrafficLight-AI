using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Enable AI Command", menuName = "Utilities/DevConsole/Commands/EnableAI")]
public class EnableAICommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        if (bool.TryParse(args[0], out bool enable))
        {
            SimulationController.instance.isAIControlled = enable;
            SimulationController.instance.ResetSimulation(SimulationController.instance.seed + 1);
            return true;
        }
        return false;
    }
}