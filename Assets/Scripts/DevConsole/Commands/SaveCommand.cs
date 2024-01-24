using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Save Command", menuName = "Utilities/DevConsole/Commands/Save")]
public class SaveCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        if (args.Length != 0 && args[0].All(char.IsLetterOrDigit))
        {
            SimulationController.instance.SaveNN(args[0]);
            return true;
        }
        return false;
    }
}