using System.Threading.Tasks;

namespace Webmilio.Commons.Console;

public interface ICommand
{
    public Task Execute(string input, string calledName, string[] args);

    public string Describe();

    public string Name { get; }
    public string[] Aliases { get; }
}

public abstract class Command : ICommand
{
    public abstract Task Execute(string input, string calledName, string[] args);

    public virtual string Describe()
    {
        return string.Empty;
    }

    public abstract string Name { get; }
}