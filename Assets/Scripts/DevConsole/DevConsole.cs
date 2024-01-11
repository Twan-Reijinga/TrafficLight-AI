using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DevConsole
{
    private readonly IEnumerable<IConsoleCommand> commands;
    public DevConsole(IEnumerable<IConsoleCommand> commands)
    {
        this.commands = commands;
    }

    public void ProcessCommand(string input)
    {
        string[] inputSplit = input.Split(' ');
        string commandInput = inputSplit[0];
        string[] args = inputSplit.Skip(1).ToArray();
        foreach (IConsoleCommand command in commands)
        {
            if (!(commandInput == command.Operation))
            {
                continue;
            }
            if (command.Execute(args))
            {
                return;
            }
        }
    }
}
