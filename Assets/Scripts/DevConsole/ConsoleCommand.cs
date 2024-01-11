using UnityEngine;

public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
{
    [SerializeField]
    private string operation = string.Empty;
    public string Operation => operation;

    public abstract bool Execute(string[] args);
}
