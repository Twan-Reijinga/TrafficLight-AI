public interface IConsoleCommand
{
    public string Operation { get; }
    public bool Execute(string[] args);
}
