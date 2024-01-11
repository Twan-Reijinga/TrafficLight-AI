using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Log Command", menuName = "Utilities/DevConsole/Commands/Log")]
public class LogCommand : ConsoleCommand
{
    public override bool Execute(string[] args)
    {
        Debug.Log(string.Join(" ", args));
        return true;
    }
}