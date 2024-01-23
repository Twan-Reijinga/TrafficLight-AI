using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Reset Command", menuName = "Utilities/DevConsole/Commands/Reset")]
public class ResetCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        int seed;
        if (args.Length == 0 || !int.TryParse(args[0], out seed))
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }
        SimulationController.instance.ResetSimulation(seed);
        return true;
    }
}