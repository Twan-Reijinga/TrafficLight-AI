using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Load Command", menuName = "Utilities/DevConsole/Commands/Load")]
public class LoadCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        if (args.Length != 0 && args[0].All(char.IsLetterOrDigit))
        {
            SimulationController.instance.LoadNN(args[0], (args.Length != 1 && int.TryParse(args[1], out int seed)) ? seed : 0);
            return true;
        }
        return false;
    }
}